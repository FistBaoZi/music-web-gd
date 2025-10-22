<template>
  <div class="favorites-view">
    <ParticleBackground />
    <div class="favorites-header">
      <h2>我的收藏</h2>
      <div class="header-actions">
        <div class="search-box" v-if="favorites.length > 0">
          <i class="fas fa-search"></i>
          <input 
            type="text" 
            v-model="searchKeyword" 
            placeholder="搜索收藏..."
            @input="filterFavorites"
          >
          <i 
            v-if="searchKeyword" 
            class="fas fa-times clear-icon" 
            @click="clearSearch"
          ></i>
        </div>
        <span>共 {{ filteredFavorites.length }} 首收藏</span>
      </div>
    </div>

    <div v-if="favorites.length === 0" class="empty">
      <i class="fas fa-heart-broken"></i>
      <p>暂无收藏歌曲</p>
      <p class="hint">在播放列表中点击爱心收藏你喜欢的歌曲</p>
    </div>

    <div v-else class="song-list">
      <div 
        v-for="(song, index) in filteredFavorites" 
        :key="song.id + song.source"
        class="song-item"
        :class="{ playing: isCurrentSong(song), loading: loadingSongIndex === index }"
        @dblclick="playSong(song)"
      >
        <div class="song-index">
          <span v-if="!isCurrentSong(song) && loadingSongIndex !== index">{{ index + 1 }}</span>
          <i v-else-if="loadingSongIndex === index" class="fas fa-spinner fa-spin loading-icon"></i>
          <i v-else class="fas fa-volume-up playing-icon"></i>
        </div>
        <div class="song-info">
          <div class="song-name">{{ song.name }}</div>
          <div class="song-artist">{{ song.artist?.join(' / ') }}</div>
        </div>
        <div class="song-album">{{ song.album }}</div>
        <div class="song-duration">
          <span v-if="song.size">{{ formatSize(song.size) }}</span>
        </div>
        <div class="song-actions">
          <button @click="playSong(song)" title="播放">
            <i class="fas fa-play"></i>
          </button>
          <button @click="addToPlaylist(song)" title="添加到播放列表" class="playlist-btn">
            <i class="fas fa-plus"></i>
          </button>
          <button @click="downloadSong(song)" title="下载" class="download-btn">
            <i class="fas fa-download"></i>
          </button>
          <button @click="removeFromFavorites(song)" title="取消收藏" class="unfavorite-btn">
            <i class="fas fa-heart-broken"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import { useMusicStore } from '../stores/music'
import ParticleBackground from '../components/ParticleBackground.vue'
import toast from '../utils/toast'

const musicStore = useMusicStore()

const favorites = computed(() => musicStore.favorites)
const currentSong = computed(() => musicStore.currentSong)

const loadingSongIndex = ref(null)
const searchKeyword = ref('')
const filteredFavorites = ref([])

// 初始化过滤列表
filteredFavorites.value = favorites.value

const playSong = async (song) => {
  const index = filteredFavorites.value.findIndex(
    s => s.id === song.id && s.source === song.source
  )
  loadingSongIndex.value = index
  try {
    await musicStore.playSong(song)
  } finally {
    loadingSongIndex.value = null
  }
}

const addToPlaylist = (song) => {
  const exists = musicStore.playlist.find(
    item => item.id === song.id && item.source === song.source
  )
  if (!exists) {
    musicStore.playlist.push(song)
    toast.success(`已添加到播放列表: ${song.name}`)
  } else {
    toast.info('歌曲已在播放列表中')
  }
}

const removeFromFavorites = (song) => {
  musicStore.removeFromFavorites(song)
  toast.info(`已取消收藏: ${song.name}`)
}

const downloadSong = async (song) => {
  try {
    // 如果歌曲已经有URL，直接下载
    if (song.url) {
      downloadFromUrl(song.url, `${song.name} - ${song.artist?.join(', ')}.mp3`)
      toast.success(`正在下载: ${song.name}`)
      return
    }

    // 否则先获取URL
    const musicApi = await import('../api/music')
    const urlData = await musicApi.getSongUrl(song.id, song.source || 'netease', 320)
    
    if (!urlData.url) {
      toast.error('无法获取歌曲下载链接')
      return
    }

    downloadFromUrl(urlData.url, `${song.name} - ${song.artist?.join(', ')}.mp3`)
    toast.success(`正在下载: ${song.name}`)
  } catch (error) {
    console.error('下载失败:', error)
    toast.error('下载失败，请稍后重试')
  }
}

const downloadFromUrl = (url, filename) => {
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

const formatSize = (sizeInKB) => {
  const kb = parseFloat(sizeInKB)
  if (isNaN(kb)) return '--'
  
  if (kb < 1024) {
    return kb.toFixed(2) + ' KB'
  } else {
    const mb = kb / 1024
    return mb.toFixed(2) + ' MB'
  }
}

// 过滤收藏列表
const filterFavorites = () => {
  if (!searchKeyword.value.trim()) {
    filteredFavorites.value = favorites.value
    return
  }
  
  const keyword = searchKeyword.value.toLowerCase()
  filteredFavorites.value = favorites.value.filter(song => {
    return song.name.toLowerCase().includes(keyword) ||
           song.artist?.some(artist => artist.toLowerCase().includes(keyword)) ||
           song.album?.toLowerCase().includes(keyword)
  })
}

// 清除搜索
const clearSearch = () => {
  searchKeyword.value = ''
  filterFavorites()
}

// 检查是否是当前播放的歌曲
const isCurrentSong = (song) => {
  return currentSong.value && 
         currentSong.value.id === song.id && 
         currentSong.value.source === song.source
}

// 监听收藏列表变化，更新过滤列表
watch(favorites, () => {
  filterFavorites()
}, { deep: true })
</script>

<style scoped>
.favorites-view {
  padding: 30px;
  position: relative;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.favorites-header {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  flex-shrink: 0;
}

.favorites-header h2 {
  font-size: 28px;
  color: #ccd6f6;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

.search-box {
  position: relative;
  display: flex;
  align-items: center;
  background: rgba(30, 30, 50, 0.4);
  border: 1px solid rgba(100, 255, 218, 0.2);
  border-radius: 20px;
  padding: 8px 16px;
  gap: 10px;
  transition: all 0.3s;
}

.search-box:focus-within {
  border-color: rgba(100, 255, 218, 0.5);
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.2);
}

.search-box i {
  color: #8892b0;
  font-size: 14px;
}

.search-box input {
  background: transparent;
  border: none;
  outline: none;
  color: #ccd6f6;
  font-size: 14px;
  width: 200px;
}

.search-box input::placeholder {
  color: #8892b0;
}

.clear-icon {
  cursor: pointer;
  transition: color 0.3s;
}

.clear-icon:hover {
  color: #64ffda;
}

.header-actions span {
  color: #8892b0;
  font-size: 14px;
}

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  color: #8892b0;
  flex: 1;
}

.empty i {
  font-size: 64px;
  margin-bottom: 20px;
  color: rgba(255, 107, 129, 0.3);
}

.empty .hint {
  font-size: 12px;
  color: #8892b0;
  margin-top: 10px;
}

.song-list {
  background: rgba(30, 30, 50, 0.4);
  border-radius: 12px;
  overflow-y: auto;
  overflow-x: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
  flex: 1;
  height: 100%;
}

.song-list::-webkit-scrollbar {
  width: 8px;
}

.song-list::-webkit-scrollbar-track {
  background: rgba(30, 30, 50, 0.4);
  border-radius: 4px;
}

.song-list::-webkit-scrollbar-thumb {
  background: rgba(100, 255, 218, 0.3);
  border-radius: 4px;
}

.song-list::-webkit-scrollbar-thumb:hover {
  background: rgba(100, 255, 218, 0.5);
}

.song-item {
  display: flex;
  align-items: center;
  padding: 15px 20px;
  border-bottom: 1px solid rgba(100, 255, 218, 0.05);
  transition: all 0.3s;
  cursor: pointer;
}

.song-item:hover {
  background: rgba(100, 255, 218, 0.05);
}

.song-item.playing {
  background: linear-gradient(90deg, rgba(100, 255, 218, 0.15), transparent);
}

.song-index {
  width: 40px;
  text-align: center;
  color: #8892b0;
  font-size: 14px;
}

.playing-icon {
  color: #64ffda;
  animation: pulse 1.5s ease-in-out infinite;
}

.loading-icon {
  color: #00d4ff;
}

.song-item.loading {
  background: linear-gradient(90deg, rgba(100, 255, 218, 0.05), transparent);
  pointer-events: none;
  opacity: 0.7;
}

.song-item.loading .song-actions {
  opacity: 0.5;
  pointer-events: none;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.song-info {
  flex: 2;
  min-width: 0;
  margin-right: 20px;
}

.song-name {
  font-size: 15px;
  font-weight: 500;
  color: #ccd6f6;
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.song-artist {
  font-size: 13px;
  color: #8892b0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.song-album {
  flex: 1;
  color: #8892b0;
  font-size: 13px;
  margin-right: 20px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.song-duration {
  width: 80px;
  text-align: right;
  color: #8892b0;
  font-size: 13px;
}

.song-actions {
  display: flex;
  gap: 10px;
  opacity: 1;
  transition: opacity 0.3s;
  margin-left: 20px;
}

.song-item:hover .song-actions button {
  transform: scale(1.05);
}

.song-actions button {
  width: 32px;
  height: 32px;
  border: none;
  background: rgba(100, 255, 218, 0.2);
  color: #64ffda;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  font-size: 13px;
  border: 1px solid rgba(100, 255, 218, 0.3);
}

.song-item:hover .song-actions button {
  background: rgba(100, 255, 218, 0.3);
  transform: scale(1.05);
}

.song-actions button:hover {
  background: rgba(100, 255, 218, 0.4) !important;
  transform: scale(1.15) !important;
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.5);
}

.song-actions button.playlist-btn {
  background: rgba(100, 200, 255, 0.2);
  color: #64c8ff;
  border: 1px solid rgba(100, 200, 255, 0.3);
}

.song-item:hover .song-actions button.playlist-btn {
  background: rgba(100, 200, 255, 0.3);
}

.song-actions button.playlist-btn:hover {
  background: rgba(100, 200, 255, 0.4) !important;
  box-shadow: 0 0 15px rgba(100, 200, 255, 0.5);
}

.song-actions button.download-btn {
  background: rgba(0, 212, 255, 0.2);
  color: #00d4ff;
  border: 1px solid rgba(0, 212, 255, 0.3);
}

.song-item:hover .song-actions button.download-btn {
  background: rgba(0, 212, 255, 0.3);
}

.song-actions button.download-btn:hover {
  background: rgba(0, 212, 255, 0.4) !important;
  box-shadow: 0 0 15px rgba(0, 212, 255, 0.5);
}

.song-actions button.unfavorite-btn {
  background: rgba(255, 71, 87, 0.2);
  color: #ff4757;
  border: 1px solid rgba(255, 71, 87, 0.3);
}

.song-item:hover .song-actions button.unfavorite-btn {
  background: rgba(255, 71, 87, 0.3);
}

.song-actions button.unfavorite-btn:hover {
  background: rgba(255, 71, 87, 0.4) !important;
  box-shadow: 0 0 15px rgba(255, 71, 87, 0.5);
}
</style>
