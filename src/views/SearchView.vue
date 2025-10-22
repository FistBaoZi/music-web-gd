<template>
  <div class="search-view">
    <ParticleBackground />
    <div class="search-header">
      <h2>搜索结果</h2>
      <p v-if="searchResults.length > 0">找到 {{ searchResults.length }} 首歌曲 · 双击歌曲播放</p>
    </div>

    <div v-if="loading && searchResults.length === 0" class="loading">
      <i class="fas fa-spinner fa-spin"></i>
      <p>加载中...</p>
    </div>

    <div v-else-if="searchResults.length === 0" class="empty">
      <i class="fas fa-search"></i>
      <p>暂无搜索结果</p>
      <p class="hint">在顶部搜索框输入歌曲名、歌手或专辑</p>
    </div>

    <div v-else class="song-list">
      <div 
        v-for="(song, index) in searchResults" 
        :key="song.id + song.source"
        class="song-item"
        :class="{ playing: currentSong?.id === song.id, loading: loadingSongId === song.id }"
        @dblclick="handlePlay(song)"
      >
        <div class="song-index">
          <span v-if="currentSong?.id !== song.id && loadingSongId !== song.id">{{ index + 1 }}</span>
          <i v-else-if="loadingSongId === song.id" class="fas fa-spinner fa-spin loading-icon"></i>
          <i v-else class="fas fa-volume-up playing-icon"></i>
        </div>
        <div class="song-info">
          <div class="song-name">{{ song.name }}</div>
          <div class="song-artist">{{ song.artist?.join(' / ') }}</div>
        </div>
        <div class="song-album">{{ song.album }}</div>
        <div class="song-actions">
          <button @click="handlePlay(song)" title="播放">
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
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useMusicStore } from '../stores/music'
import { useAppStore } from '../stores/app'
import ParticleBackground from '../components/ParticleBackground.vue'

const musicStore = useMusicStore()
const appStore = useAppStore()

const searchResults = computed(() => musicStore.searchResults)
const currentSong = computed(() => musicStore.currentSong)
const loading = computed(() => musicStore.loading)
const showLyrics = computed(() => musicStore.showLyrics)

const loadingSongId = ref(null)

const handlePlay = async (song) => {
  loadingSongId.value = song.id
  try {
    await musicStore.playSong(song)
    // 播放歌曲后可选择自动跳转到歌词页面
    // appStore.setCurrentView('lyrics')
  } finally {
    loadingSongId.value = null
  }
}

const addToPlaylist = (song) => {
  if (!musicStore.playlist.find(item => item.id === song.id)) {
    musicStore.playlist.push(song)
  }
}

const downloadSong = async (song) => {
  try {
    // 获取歌曲URL
    const musicApi = await import('../api/music')
    const urlData = await musicApi.getSongUrl(song.id, song.source || musicStore.currentSource, 320)
    
    if (!urlData.url) {
      alert('无法获取歌曲下载链接')
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
    
    console.log(`正在下载: ${song.name}`)
  } catch (error) {
    console.error('下载失败:', error)
    alert('下载失败，请稍后重试')
  }
}
</script>

<style scoped>
.search-view {
  padding: 30px;
  position: relative;
  min-height: 100%;
}

.search-header {
  margin-bottom: 30px;
  position: relative;
  z-index: 1;
}

.search-header h2 {
  font-size: 28px;
  color: #ccd6f6;
  margin-bottom: 10px;
}

.search-header p {
  color: #8892b0;
  font-size: 14px;
}

.loading, .empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  color: #999;
}

.loading i, .empty i {
  font-size: 64px;
  margin-bottom: 20px;
  color: #ddd;
}

.empty .hint {
  font-size: 12px;
  color: #bbb;
  margin-top: 10px;
}

.song-list {
  background: rgba(30, 30, 50, 0.4);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
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
  color: #64ffda;
}

.song-item.loading {
  background: linear-gradient(90deg, rgba(102, 126, 234, 0.05), transparent);
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
  background: rgba(100, 255, 218, 0.8);
  color: #0f0c29;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  font-size: 13px;
}

.song-item:hover .song-actions button {
  background: rgba(100, 255, 218, 0.9);
  transform: scale(1.05);
}

.song-actions button:hover {
  background: #64ffda !important;
  transform: scale(1.15) !important;
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.6);
}

.song-actions button.download-btn {
  background: rgba(0, 212, 255, 0.8);
}

.song-item:hover .song-actions button.download-btn {
  background: rgba(0, 212, 255, 0.9);
}

.song-actions button.download-btn:hover {
  background: #00d4ff !important;
  box-shadow: 0 0 15px rgba(0, 212, 255, 0.6);
}
</style>
