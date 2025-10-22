<template>
  <div class="player" v-if="currentSong">
    <div class="song-info">
      <img :src="currentSong.pic" alt="专辑图" class="album-cover">
      <div class="info">
        <div class="song-name">{{ currentSong.name }}</div>
        <div class="artist">{{ currentSong.artist?.join(', ') }}</div>
      </div>
    </div>

    <div class="player-controls">
      <div class="control-buttons">
        <button @click="previousSong" title="上一曲">
          <i class="fas fa-step-backward"></i>
        </button>
        <button @click="togglePlay" class="play-btn" :title="isPlaying ? '暂停' : '播放'">
          <i :class="isPlaying ? 'fas fa-pause' : 'fas fa-play'"></i>
        </button>
        <button @click="nextSong" title="下一曲">
          <i class="fas fa-step-forward"></i>
        </button>
      </div>

      <div class="progress-bar">
        <span class="time">{{ formatTime(currentTime) }}</span>
        <div class="progress" @click="seek">
          <div class="progress-fill" :style="{ width: progress + '%' }"></div>
        </div>
        <span class="time">{{ formatTime(duration) }}</span>
      </div>
    </div>

    <div class="player-extra">
      <button @click="downloadCurrentSong" title="下载当前歌曲" class="download-btn">
        <i class="fas fa-download"></i>
      </button>
      <button @click="goToLyrics" title="查看歌词">
        <i class="fas fa-align-left"></i>
      </button>
      <button @click="toggleLyrics" :class="{ active: showLyrics }" title="歌词面板">
        <i class="fas fa-comment-dots"></i>
      </button>
      <button @click="cyclePlayMode" :title="playModeText">
        <i v-if="playMode !== 'single'" :class="playModeIcon"></i>
        <span v-else class="single-mode-icon">
          <i class="fas fa-redo"></i>
          <span class="mode-number">1</span>
        </span>
      </button>
      <div class="volume-control">
        <i class="fas fa-volume-up"></i>
        <input 
          type="range" 
          min="0" 
          max="100" 
          v-model="volume"
          @input="updateVolume"
        >
      </div>
    </div>

    <audio 
      ref="audioPlayer"
      @timeupdate="updateTime"
      @loadedmetadata="updateDuration"
      @ended="handleSongEnd"
    ></audio>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useMusicStore } from '../stores/music'
import { useAppStore } from '../stores/app'

const musicStore = useMusicStore()
const appStore = useAppStore()

const audioPlayer = ref(null)
const isPlaying = ref(false)
const currentTime = ref(0)
const duration = ref(0)
const volume = ref(parseInt(localStorage.getItem('volume')) || 80)
const playMode = ref(localStorage.getItem('playMode') || 'loop') // loop, random, single

const currentSong = computed(() => musicStore.currentSong)
const showLyrics = computed(() => musicStore.showLyrics)
const progress = computed(() => {
  return duration.value ? (currentTime.value / duration.value) * 100 : 0
})

const playModeIcon = computed(() => {
  const icons = {
    loop: 'fas fa-redo',
    random: 'fas fa-random',
    single: 'fas fa-redo'
  }
  return icons[playMode.value]
})

const playModeText = computed(() => {
  const texts = {
    loop: '列表循环',
    random: '随机播放',
    single: '单曲循环'
  }
  return texts[playMode.value]
})

watch(currentSong, async (newSong, oldSong) => {
  if (newSong && newSong.url) {
    // 检查是否是同一首歌（刷新页面后恢复播放）
    const isSameSong = oldSong && oldSong.id === newSong.id && oldSong.source === newSong.source
    const savedTime = parseFloat(localStorage.getItem('currentPlayTime')) || 0
    
    audioPlayer.value.src = newSong.url
    
    if (isSameSong && savedTime > 0) {
      // 恢复之前的播放进度
      audioPlayer.value.currentTime = savedTime
    }
    
    await audioPlayer.value.play()
    isPlaying.value = true
  }
})

const togglePlay = () => {
  if (isPlaying.value) {
    audioPlayer.value.pause()
  } else {
    audioPlayer.value.play()
  }
  isPlaying.value = !isPlaying.value
}

const previousSong = () => {
  musicStore.playPrevious()
}

const nextSong = () => {
  if (playMode.value === 'random') {
    musicStore.playRandom()
  } else {
    musicStore.playNext()
  }
}

const updateTime = () => {
  currentTime.value = audioPlayer.value.currentTime
  // 更新歌词位置
  updateCurrentLyric(currentTime.value)
  // 保存当前播放时间
  localStorage.setItem('currentPlayTime', currentTime.value)
}

const updateDuration = () => {
  duration.value = audioPlayer.value.duration
}

// 更新当前歌词
const updateCurrentLyric = (time) => {
  // 通过事件通知歌词组件更新
  window.dispatchEvent(new CustomEvent('lyric-time-update', { 
    detail: { currentTime: time } 
  }))
}

const seek = (e) => {
  const rect = e.currentTarget.getBoundingClientRect()
  const percent = (e.clientX - rect.left) / rect.width
  audioPlayer.value.currentTime = duration.value * percent
}

const updateVolume = () => {
  audioPlayer.value.volume = volume.value / 100
  localStorage.setItem('volume', volume.value)
}

const toggleLyrics = () => {
  musicStore.toggleLyrics()
}

const goToLyrics = () => {
  appStore.setCurrentView('lyrics')
}

const downloadCurrentSong = () => {
  if (!currentSong.value) {
    alert('当前没有播放歌曲')
    return
  }

  if (!currentSong.value.url) {
    alert('无法获取歌曲下载链接')
    return
  }

  const link = document.createElement('a')
  link.href = currentSong.value.url
  link.download = `${currentSong.value.name} - ${currentSong.value.artist?.join(', ')}.mp3`
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  console.log(`正在下载: ${currentSong.value.name}`)
}

const cyclePlayMode = () => {
  const modes = ['loop', 'random', 'single']
  const currentIndex = modes.indexOf(playMode.value)
  playMode.value = modes[(currentIndex + 1) % modes.length]
  localStorage.setItem('playMode', playMode.value)
}

const handleSongEnd = () => {
  if (playMode.value === 'single') {
    audioPlayer.value.currentTime = 0
    audioPlayer.value.play()
  } else {
    nextSong()
  }
}

const formatTime = (seconds) => {
  if (!seconds || isNaN(seconds)) return '00:00'
  const mins = Math.floor(seconds / 60)
  const secs = Math.floor(seconds % 60)
  return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
}

// 初始化音量
if (audioPlayer.value) {
  audioPlayer.value.volume = volume.value / 100
}

// 处理歌词跳转事件
const handleSeekToTime = (event) => {
  if (audioPlayer.value && event.detail && event.detail.time !== undefined) {
    audioPlayer.value.currentTime = event.detail.time
    // 如果当前是暂停状态，也开始播放
    if (!isPlaying.value) {
      audioPlayer.value.play()
      isPlaying.value = true
    }
  }
}

onMounted(() => {
  // 页面加载时恢复音量
  if (audioPlayer.value) {
    audioPlayer.value.volume = volume.value / 100
  }
  
  // 如果有保存的歌曲，尝试恢复播放（但不自动播放，需要用户交互）
  if (currentSong.value && currentSong.value.url) {
    audioPlayer.value.src = currentSong.value.url
    const savedTime = parseFloat(localStorage.getItem('currentPlayTime')) || 0
    if (savedTime > 0) {
      audioPlayer.value.currentTime = savedTime
    }
  }
  
  // 监听歌词跳转事件
  window.addEventListener('seek-to-time', handleSeekToTime)
})

onUnmounted(() => {
  // 移除歌词跳转事件监听
  window.removeEventListener('seek-to-time', handleSeekToTime)
})
</script>

<style scoped>
.player {
  height: 90px;
  background: rgba(15, 12, 41, 0.95);
  backdrop-filter: blur(10px);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 30px;
  color: #ccd6f6;
  border-top: 1px solid rgba(100, 255, 218, 0.2);
  box-shadow: 0 -4px 20px rgba(0, 0, 0, 0.3);
}

.song-info {
  display: flex;
  align-items: center;
  gap: 15px;
  flex: 1;
  min-width: 250px;
}

.album-cover {
  width: 60px;
  height: 60px;
  border-radius: 8px;
  object-fit: cover;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.info {
  overflow: hidden;
}

.song-name {
  font-size: 16px;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  margin-bottom: 5px;
}

.artist {
  font-size: 13px;
  color: #8892b0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.player-controls {
  flex: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  min-width: 400px;
}

.control-buttons {
  display: flex;
  align-items: center;
  gap: 20px;
}

.control-buttons button {
  background: transparent;
  border: none;
  color: #ccd6f6;
  font-size: 18px;
  cursor: pointer;
  transition: all 0.3s;
  padding: 8px;
}

.control-buttons button:hover {
  color: #64ffda;
  transform: scale(1.1);
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.play-btn {
  width: 45px;
  height: 45px;
  border-radius: 50%;
  background: linear-gradient(135deg, #64ffda, #00d4ff) !important;
  color: #0f0c29 !important;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px !important;
  box-shadow: 0 0 20px rgba(100, 255, 218, 0.4);
}

.play-btn:hover {
  background: linear-gradient(135deg, #00d4ff, #64ffda) !important;
  color: #0f0c29 !important;
  box-shadow: 0 0 25px rgba(100, 255, 218, 0.6);
}

.progress-bar {
  display: flex;
  align-items: center;
  gap: 15px;
  width: 100%;
}

.time {
  font-size: 12px;
  color: #8892b0;
  min-width: 45px;
  text-align: center;
}

.progress {
  flex: 1;
  height: 6px;
  background: rgba(100, 255, 218, 0.1);
  border-radius: 3px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #64ffda, #00d4ff);
  border-radius: 3px;
  transition: width 0.1s;
  box-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.player-extra {
  display: flex;
  align-items: center;
  gap: 15px;
  flex: 1;
  justify-content: flex-end;
  min-width: 250px;
}

.player-extra button {
  background: transparent;
  border: none;
  color: #8892b0;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s;
  padding: 8px;
}

.player-extra button:hover,
.player-extra button.active {
  color: #64ffda;
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.player-extra button.download-btn {
  color: #64ffda;
}

.player-extra button.download-btn:hover {
  color: #00d4ff;
  transform: scale(1.2);
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.single-mode-icon {
  position: relative;
  display: inline-block;
}

.single-mode-icon .mode-number {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 10px;
  font-weight: bold;
  color: inherit;
}

.volume-control {
  display: flex;
  align-items: center;
  gap: 10px;
}

.volume-control input {
  width: 80px;
  cursor: pointer;
}
</style>
