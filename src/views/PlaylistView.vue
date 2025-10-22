<template>
  <div class="playlist-view">
    <ParticleBackground />
    <div class="playlist-header">
      <h2>播放列表</h2>
      <div class="header-actions">
        <span>共 {{ playlist.length }} 首歌曲</span>
        <button @click="clearPlaylist" v-if="playlist.length > 0">
          <i class="fas fa-trash"></i> 清空列表
        </button>
      </div>
    </div>

    <div v-if="playlist.length === 0" class="empty">
      <i class="fas fa-list-music"></i>
      <p>播放列表为空</p>
      <p class="hint">搜索并添加你喜欢的歌曲</p>
    </div>

    <div v-else class="song-list">
      <div 
        v-for="(song, index) in playlist" 
        :key="index"
        class="song-item"
        :class="{ playing: currentIndex === index, loading: loadingSongIndex === index }"
        @dblclick="playSong(song, index)"
      >
        <div class="song-index">
          <span v-if="currentIndex !== index && loadingSongIndex !== index">{{ index + 1 }}</span>
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
          <button @click="playSong(song, index)" title="播放">
            <i class="fas fa-play"></i>
          </button>
          <button @click="downloadSong(song)" title="下载" class="download-btn">
            <i class="fas fa-download"></i>
          </button>
          <button @click="removeFromPlaylist(index)" title="移除">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- 清空确认对话框 -->
    <div v-if="showClearConfirm" class="confirm-overlay" @click="cancelClearPlaylist">
      <div class="confirm-dialog" @click.stop>
        <div class="confirm-header">
          <i class="fas fa-exclamation-triangle"></i>
          <h3>确认清空</h3>
        </div>
        <p class="confirm-message">确定要清空播放列表吗？此操作不可撤销。</p>
        <div class="confirm-actions">
          <button @click="cancelClearPlaylist" class="cancel-btn">取消</button>
          <button @click="confirmClearPlaylist" class="confirm-btn">确认清空</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useMusicStore } from '../stores/music'
import ParticleBackground from '../components/ParticleBackground.vue'
import toast from '../utils/toast'

const musicStore = useMusicStore()

const playlist = computed(() => musicStore.playlist)
const currentIndex = computed(() => musicStore.currentIndex)

const loadingSongIndex = ref(null)
const showClearConfirm = ref(false)

const playSong = async (song, index) => {
  loadingSongIndex.value = index
  try {
    await musicStore.playSong(song)
  } finally {
    loadingSongIndex.value = null
  }
}

const removeFromPlaylist = (index) => {
  musicStore.removeFromPlaylist(index)
}

const clearPlaylist = () => {
  showClearConfirm.value = true
}

const confirmClearPlaylist = () => {
  musicStore.clearPlaylist()
  showClearConfirm.value = false
  toast.success('播放列表已清空')
}

const cancelClearPlaylist = () => {
  showClearConfirm.value = false
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
  console.log(`正在下载: ${filename}`)
}

const formatSize = (sizeInKB) => {
  // API 返回的 size 单位是 KB
  const kb = parseFloat(sizeInKB)
  if (isNaN(kb)) return '--'
  
  if (kb < 1024) {
    return kb.toFixed(2) + ' KB'
  } else {
    const mb = kb / 1024
    return mb.toFixed(2) + ' MB'
  }
}
</script>

<style scoped>
.playlist-view {
  padding: 30px;
  position: relative;
  min-height: 100%;
}

.playlist-header {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.playlist-header h2 {
  font-size: 28px;
  color: #ccd6f6;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

.header-actions span {
  color: #8892b0;
  font-size: 14px;
}

.header-actions button {
  padding: 8px 16px;
  border: none;
  background: #ff4757;
  color: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.header-actions button:hover {
  background: #e84118;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 71, 87, 0.3);
}

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  color: #8892b0;
}

.empty i {
  font-size: 64px;
  margin-bottom: 20px;
  color: rgba(100, 255, 218, 0.3);
}

.empty .hint {
  font-size: 12px;
  color: #8892b0;
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

.song-actions button:last-child {
  background: rgba(255, 71, 87, 0.8);
}

.song-item:hover .song-actions button:last-child {
  background: rgba(255, 71, 87, 0.9);
}

.song-actions button:last-child:hover {
  background: #e84118 !important;
  box-shadow: 0 2px 8px rgba(255, 71, 87, 0.4);
}

/* 确认对话框样式 */
.confirm-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.confirm-dialog {
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  border-radius: 16px;
  padding: 30px;
  max-width: 400px;
  width: 90%;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(100, 255, 218, 0.2);
  animation: slideIn 0.3s;
}

@keyframes slideIn {
  from {
    transform: translateY(-20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.confirm-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
}

.confirm-header i {
  font-size: 24px;
  color: #ffa502;
}

.confirm-header h3 {
  font-size: 20px;
  color: #ccd6f6;
  margin: 0;
}

.confirm-message {
  color: #8892b0;
  font-size: 15px;
  line-height: 1.6;
  margin-bottom: 25px;
}

.confirm-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.confirm-actions button {
  padding: 10px 24px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.cancel-btn {
  background: rgba(100, 255, 218, 0.1);
  color: #64ffda;
  border: 1px solid rgba(100, 255, 218, 0.3);
}

.cancel-btn:hover {
  background: rgba(100, 255, 218, 0.2);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(100, 255, 218, 0.2);
}

.confirm-btn {
  background: #ff4757;
  color: white;
}

.confirm-btn:hover {
  background: #e84118;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 71, 87, 0.4);
}
</style>
