<template>
  <div class="lyrics-view">
    <ParticleBackground />
    <div v-if="!currentSong" class="no-song">
      <i class="fas fa-music"></i>
      <h3>暂无播放音乐</h3>
      <p>搜索并播放歌曲后，歌词将在这里显示</p>
    </div>

    <div v-else class="lyrics-container">
      <!-- 歌曲信息 -->
      <div class="song-header">
        <div class="album-art">
          <img :src="currentSong.pic || '/placeholder.png'" :alt="currentSong.name">
          <div class="playing-indicator">
            <div class="bar"></div>
            <div class="bar"></div>
            <div class="bar"></div>
            <div class="bar"></div>
          </div>
        </div>
        <div class="song-meta">
          <h2>{{ currentSong.name }}</h2>
          <p class="artist">{{ currentSong.artist?.join(' / ') }}</p>
          <p class="album">{{ currentSong.album }}</p>
          <button @click="downloadSong" class="download-song-btn">
            <i class="fas fa-download"></i> 下载歌曲
          </button>
        </div>
      </div>

      <!-- 歌词显示 -->
      <div class="lyrics-scroll" ref="lyricsScroll">
        <div v-if="parsedLyrics.length === 0" class="no-lyrics">
          <i class="fas fa-file-audio"></i>
          <p>暂无歌词</p>
        </div>
        <div v-else class="lyrics-lines">
          <div 
            v-for="(line, index) in parsedLyrics" 
            :key="index"
            class="lyric-line"
            :class="{ 
              active: currentLyricIndex === index,
              passed: index < currentLyricIndex 
            }"
            :ref="el => { if (el && currentLyricIndex === index) scrollToLyric(el) }"
          >
            <div class="lyric-time">{{ formatLyricTime(line.time) }}</div>
            <div class="lyric-content">
              <p class="original">{{ line.text }}</p>
              <p v-if="line.translation" class="translation">{{ line.translation }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useMusicStore } from '../stores/music'
import ParticleBackground from '../components/ParticleBackground.vue'

const musicStore = useMusicStore()
const lyricsScroll = ref(null)
const currentLyricIndex = ref(0)
const currentTime = ref(0)

const currentSong = computed(() => musicStore.currentSong)
const parsedLyrics = computed(() => musicStore.parsedLyrics)

// 监听播放时间更新
const handleTimeUpdate = (event) => {
  currentTime.value = event.detail.currentTime
  updateCurrentLyric(currentTime.value)
}

// 更新当前歌词高亮
const updateCurrentLyric = (time) => {
  if (parsedLyrics.value.length === 0) return
  
  for (let i = parsedLyrics.value.length - 1; i >= 0; i--) {
    if (time >= parsedLyrics.value[i].time) {
      if (currentLyricIndex.value !== i) {
        currentLyricIndex.value = i
      }
      break
    }
  }
}

// 滚动到当前歌词
const scrollToLyric = (el) => {
  if (el && lyricsScroll.value) {
    setTimeout(() => {
      el.scrollIntoView({ behavior: 'smooth', block: 'center' })
    }, 100)
  }
}

// 格式化歌词时间
const formatLyricTime = (seconds) => {
  if (!seconds || isNaN(seconds)) return '00:00'
  const mins = Math.floor(seconds / 60)
  const secs = Math.floor(seconds % 60)
  return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
}

const downloadSong = () => {
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

onMounted(() => {
  window.addEventListener('lyric-time-update', handleTimeUpdate)
})

onUnmounted(() => {
  window.removeEventListener('lyric-time-update', handleTimeUpdate)
})
</script>

<style scoped>
.lyrics-view {
  height: 100%;
  overflow: hidden;
}

.no-song {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
  padding: 40px;
}

.no-song i {
  font-size: 80px;
  margin-bottom: 30px;
  color: #ddd;
}

.no-song h3 {
  font-size: 24px;
  margin-bottom: 10px;
  color: #666;
}

.no-song p {
  font-size: 14px;
  color: #999;
}

.lyrics-container {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.song-header {
  display: flex;
  align-items: center;
  padding: 40px;
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  color: #ccd6f6;
  gap: 30px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.album-art {
  position: relative;
  width: 180px;
  height: 180px;
  flex-shrink: 0;
}

.album-art img {
  width: 100%;
  height: 100%;
  border-radius: 15px;
  object-fit: cover;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
}

.playing-indicator {
  position: absolute;
  bottom: 15px;
  right: 15px;
  display: flex;
  align-items: flex-end;
  gap: 4px;
  height: 24px;
  padding: 6px 8px;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(10px);
  border-radius: 8px;
}

.bar {
  width: 3px;
  background: white;
  border-radius: 2px;
  animation: playing 0.8s ease-in-out infinite;
}

.bar:nth-child(1) { animation-delay: 0s; }
.bar:nth-child(2) { animation-delay: 0.2s; }
.bar:nth-child(3) { animation-delay: 0.4s; }
.bar:nth-child(4) { animation-delay: 0.6s; }

@keyframes playing {
  0%, 100% { height: 8px; }
  50% { height: 20px; }
}

.song-meta {
  flex: 1;
}

.song-meta h2 {
  font-size: 32px;
  margin-bottom: 12px;
  font-weight: 700;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
}

.song-meta .artist {
  font-size: 18px;
  margin-bottom: 8px;
  opacity: 0.95;
}

.song-meta .album {
  font-size: 15px;
  opacity: 0.8;
}

.download-song-btn {
  margin-top: 20px;
  padding: 12px 30px;
  background: rgba(100, 255, 218, 0.2);
  border: 2px solid #64ffda;
  color: #64ffda;
  border-radius: 25px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 10px;
}

.download-song-btn:hover {
  background: #64ffda;
  color: #0f0c29;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(100, 255, 218, 0.4);
}

.download-song-btn i {
  font-size: 16px;
}

.lyrics-scroll {
  flex: 1;
  overflow-y: auto;
  padding: 60px 40px 100px;
  background: 
    linear-gradient(rgba(15, 12, 41, 0.85), rgba(15, 12, 41, 0.95)),
    repeating-linear-gradient(
      0deg,
      rgba(100, 255, 218, 0.03) 0px,
      rgba(100, 255, 218, 0.03) 2px,
      transparent 2px,
      transparent 4px
    ),
    repeating-linear-gradient(
      90deg,
      rgba(100, 255, 218, 0.03) 0px,
      rgba(100, 255, 218, 0.03) 2px,
      transparent 2px,
      transparent 4px
    ),
    radial-gradient(
      ellipse at center,
      rgba(100, 255, 218, 0.1) 0%,
      transparent 70%
    );
  position: relative;
}

.lyrics-scroll::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: 
    radial-gradient(circle at 20% 50%, rgba(100, 255, 218, 0.1) 0%, transparent 50%),
    radial-gradient(circle at 80% 80%, rgba(0, 212, 255, 0.1) 0%, transparent 50%);
  pointer-events: none;
}

.lyrics-scroll::-webkit-scrollbar {
  width: 8px;
}

.lyrics-scroll::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.lyrics-scroll::-webkit-scrollbar-thumb {
  background: #ccc;
  border-radius: 4px;
}

.lyrics-scroll::-webkit-scrollbar-thumb:hover {
  background: #999;
}

.no-lyrics {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 100px 20px;
  color: #ccc;
}

.no-lyrics i {
  font-size: 64px;
  margin-bottom: 20px;
}

.lyrics-lines {
  max-width: 900px;
  margin: 0 auto;
  position: relative;
  z-index: 1;
}

.lyric-line {
  display: flex;
  align-items: flex-start;
  gap: 30px;
  padding: 20px 30px;
  margin-bottom: 10px;
  border-radius: 12px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  opacity: 0.4;
  transform: scale(0.95);
}

.lyric-line.passed {
  opacity: 0.5;
}

.lyric-line.active {
  opacity: 1;
  transform: scale(1.02);
  background: linear-gradient(90deg, 
    rgba(100, 255, 218, 0.2), 
    rgba(0, 212, 255, 0.1),
    transparent
  );
  box-shadow: 0 4px 20px rgba(100, 255, 218, 0.2);
  border-left: 3px solid #64ffda;
}

.lyric-time {
  min-width: 60px;
  font-size: 14px;
  color: #8892b0;
  font-weight: 500;
  padding-top: 4px;
  font-family: 'Consolas', monospace;
}

.lyric-line.active .lyric-time {
  color: #64ffda;
  font-weight: 600;
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.lyric-content {
  flex: 1;
}

.lyric-content .original {
  font-size: 22px;
  line-height: 1.8;
  color: #ccd6f6;
  margin-bottom: 8px;
  font-weight: 500;
}

.lyric-line.active .original {
  color: #64ffda;
  font-weight: 600;
  font-size: 24px;
  text-shadow: 0 0 15px rgba(100, 255, 218, 0.3);
}

.lyric-content .translation {
  font-size: 16px;
  line-height: 1.6;
  color: #8892b0;
}

.lyric-line.active .translation {
  color: #00d4ff;
  font-weight: 500;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .song-header {
    flex-direction: column;
    text-align: center;
    padding: 30px 20px;
  }

  .album-art {
    width: 140px;
    height: 140px;
  }

  .song-meta h2 {
    font-size: 24px;
  }

  .lyrics-scroll {
    padding: 30px 20px 80px;
  }

  .lyric-line {
    flex-direction: column;
    gap: 10px;
    padding: 15px 20px;
  }

  .lyric-time {
    min-width: auto;
  }

  .lyric-content .original {
    font-size: 18px;
  }

  .lyric-line.active .original {
    font-size: 20px;
  }
}
</style>
