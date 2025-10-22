<template>
  <div class="search-view">
    <ParticleBackground />
    <div class="search-header">
      <div class="header-left">
        <div class="title-with-loading">
          <h2>搜索结果</h2>
          <i v-if="searchLoading && searchResults.length > 0" class="fas fa-spinner fa-spin search-loading-icon"></i>
        </div>
        <p v-if="searchResults.length > 0 && !searchLoading">找到 {{ searchResults.length }} 首歌曲 · 双击歌曲播放</p>
        <p v-if="searchLoading && searchResults.length > 0" class="searching-text">正在搜索...</p>
      </div>
      <div class="header-actions" v-if="searchResults.length > 0">
        <button @click="addAllToPlaylist" class="add-all-btn">
          <i class="fas fa-plus-circle"></i> 全部添加到播放列表
        </button>
      </div>
    </div>

    <div v-if="searchResults.length === 0" class="empty">
      <i v-if="!searchLoading" class="fas fa-search"></i>
      <i v-else class="fas fa-spinner fa-spin"></i>
      <p v-if="!searchLoading">暂无搜索结果</p>
      <p v-else>搜索中...</p>
      <p v-if="!searchLoading" class="hint">在顶部搜索框输入歌曲名、歌手或专辑</p>
    </div>

    <div v-else class="song-list-container">
      <div class="song-list" ref="songListRef">
      <div 
        v-for="(song, index) in searchResults" 
        :key="song.id + song.source"
        class="song-item"
        :class="{ playing: currentSong?.id === song.id, loading: loadingSongIndex === index }"
        @dblclick="handlePlay(song, index)"
      >
        <div class="song-index">
          <span v-if="currentSong?.id !== song.id && loadingSongIndex !== index">{{ index + 1 }}</span>
          <i v-else-if="loadingSongIndex === index" class="fas fa-spinner fa-spin loading-icon"></i>
          <i v-else class="fas fa-volume-up playing-icon"></i>
        </div>
        <div class="song-info">
          <div class="song-name">{{ song.name }}</div>
          <div class="song-artist">{{ song.artist?.join(' / ') }}</div>
        </div>
        <div class="song-album">{{ song.album }}</div>
        <div class="song-actions">
          <button @click="handlePlay(song, index)" title="播放">
            <i class="fas fa-play"></i>
          </button>
          <button @click="addToPlaylist(song)" title="添加到播放列表">
            <i class="fas fa-plus"></i>
          </button>
          <button @click="downloadSong(song)" title="下载歌曲" class="download-btn">
            <i class="fas fa-download"></i>
          </button>
        </div>
      </div>
      
      <!-- 加载更多提示 -->
      <div v-if="searchLoading && searchResults.length > 0" class="loading-more">
        <i class="fas fa-spinner fa-spin"></i>
        <span>加载更多...</span>
      </div>
      
      <!-- 没有更多提示 -->
      <div v-if="!hasMoreResults && searchResults.length > 0 && !searchLoading" class="no-more">
        <span>已加载全部结果</span>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useMusicStore } from '../stores/music'
import { useAppStore } from '../stores/app'
import ParticleBackground from '../components/ParticleBackground.vue'
import toast from '../utils/toast'

const musicStore = useMusicStore()
const appStore = useAppStore()

const searchResults = computed(() => musicStore.searchResults)
const currentSong = computed(() => musicStore.currentSong)
const searchLoading = computed(() => musicStore.searchLoading)
const showLyrics = computed(() => musicStore.showLyrics)
const hasMoreResults = computed(() => musicStore.hasMoreResults)

const loadingSongIndex = ref(null)
const songListRef = ref(null)

const handlePlay = async (song, index) => {
  loadingSongIndex.value = index
  try {
    await musicStore.playSong(song)
    // 播放歌曲后可选择自动跳转到歌词页面
    // appStore.setCurrentView('lyrics')
  } finally {
    loadingSongIndex.value = null
  }
}

const addToPlaylist = (song) => {
  if (!musicStore.playlist.find(item => item.id === song.id)) {
    musicStore.playlist.push(song)
  }
}

const addAllToPlaylist = () => {
  let addedCount = 0
  searchResults.value.forEach(song => {
    if (!musicStore.playlist.find(item => item.id === song.id)) {
      musicStore.playlist.push(song)
      addedCount++
    }
  })
  
  if (addedCount > 0) {
    toast.success(`已添加 ${addedCount} 首歌曲到播放列表`)
  } else {
    toast.info('所有歌曲已在播放列表中')
  }
}

const downloadSong = async (song) => {
  try {
    // 获取歌曲URL
    const musicApi = await import('../api/music')
    const urlData = await musicApi.getSongUrl(song.id, song.source || musicStore.currentSource, 320)
    
    if (!urlData.url) {
      toast.error('无法获取歌曲下载链接')
      return
    }

    // 创建下载链接
    const link = document.createElement('a')
    link.href = urlData.url
    link.download = `${song.name} - ${song.artist?.join(', ')}.mp3`
    link.target = '_blank'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    
    toast.success(`正在下载: ${song.name}`)
  } catch (error) {
    console.error('下载失败:', error)
    toast.error('下载失败，请稍后重试')
  }
}

// 滚动加载处理
const handleScroll = (event) => {
  const element = event.target
  const scrollTop = element.scrollTop
  const scrollHeight = element.scrollHeight
  const clientHeight = element.clientHeight
  
  // 当滚动到距离底部100px时加载更多
  if (scrollHeight - scrollTop - clientHeight < 100 && hasMoreResults.value && !searchLoading.value) {
    musicStore.loadMoreSearchResults()
  }
}

onMounted(() => {
  // 添加滚动监听
  if (songListRef.value) {
    songListRef.value.addEventListener('scroll', handleScroll)
  }
})

onUnmounted(() => {
  // 移除滚动监听
  if (songListRef.value) {
    songListRef.value.removeEventListener('scroll', handleScroll)
  }
})
</script>

<style scoped>
.search-view {
  padding: 30px;
  position: relative;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.search-header {
  margin-bottom: 20px;
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}

.title-with-loading {
  display: flex;
  align-items: center;
  gap: 15px;
}

.header-left h2 {
  font-size: 28px;
  color: #ccd6f6;
  margin-bottom: 10px;
}

.search-loading-icon {
  font-size: 24px;
  color: #64ffda;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.header-left p {
  color: #8892b0;
  font-size: 14px;
}

.searching-text {
  color: #64ffda !important;
  font-weight: 500;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.add-all-btn {
  padding: 10px 20px;
  border: none;
  background: linear-gradient(135deg, rgba(100, 255, 218, 0.8), rgba(100, 200, 255, 0.8));
  color: #0f0c29;
  border-radius: 25px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
  box-shadow: 0 4px 15px rgba(100, 255, 218, 0.3);
}

.add-all-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(100, 255, 218, 0.5);
  background: linear-gradient(135deg, rgba(100, 255, 218, 1), rgba(100, 200, 255, 1));
}

.add-all-btn:active {
  transform: translateY(0);
}

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex: 1;
  padding: 80px 20px;
  color: #999;
}

.empty i {
  font-size: 64px;
  margin-bottom: 20px;
  color: #ddd;
}

.empty .hint {
  font-size: 12px;
  color: #bbb;
  margin-top: 10px;
}

.song-list-container {
  flex: 1;
  overflow: hidden;
  border-radius: 12px;
}

.song-list {
  background: rgba(30, 30, 50, 0.4);
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
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
  border-left: 3px solid #64ffda;
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
  flex: 1;
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

.song-actions {
  display: flex;
  gap: 10px;
  opacity: 1;
  transition: opacity 0.3s;
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

.loading-more {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 30px 20px;
  color: #64ffda;
  font-size: 14px;
}

.loading-more i {
  font-size: 18px;
}

.no-more {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 30px 20px;
  color: #8892b0;
  font-size: 13px;
}

.no-more span {
  position: relative;
  padding: 0 20px;
}

.no-more span::before,
.no-more span::after {
  content: '';
  position: absolute;
  top: 50%;
  width: 60px;
  height: 1px;
  background: linear-gradient(to right, transparent, rgba(100, 255, 218, 0.3));
}

.no-more span::before {
  right: 100%;
  margin-right: 10px;
}

.no-more span::after {
  left: 100%;
  margin-left: 10px;
  background: linear-gradient(to left, transparent, rgba(100, 255, 218, 0.3));
}
</style>
