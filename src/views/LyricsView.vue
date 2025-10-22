<template>
  <div class="lyrics-view">
    <ParticleBackground />
    <div v-if="!currentSong" class="no-song">
      <i class="fas fa-music"></i>
      <h3>暂无播放音乐</h3>
      <p>搜索并播放歌曲后，歌词将在这里显示</p>
    </div>

    <div v-else class="lyrics-container">
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
            @dblclick="seekToLyric(line.time)"
          >
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

// 跳转到指定歌词位置
const seekToLyric = (time) => {
  // 发送自定义事件，通知播放器跳转到指定时间
  window.dispatchEvent(new CustomEvent('seek-to-time', {
    detail: { time }
  }))
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
  position: relative;
  user-select: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
}

.no-song {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #999;
  padding: 40px;
  position: relative;
  z-index: 1;
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
  position: relative;
  z-index: 1;
}

.lyrics-scroll {
  flex: 1;
  overflow-y: auto;
  padding: 60px 40px 100px;
  background: transparent;
  position: relative;
}

.lyrics-scroll::-webkit-scrollbar {
  width: 8px;
}

.lyrics-scroll::-webkit-scrollbar-track {
  background: transparent;
}

.lyrics-scroll::-webkit-scrollbar-thumb {
  background: transparent;
  border-radius: 4px;
}

.lyrics-scroll::-webkit-scrollbar-thumb:hover {
  background: transparent;
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
  align-items: center;
  justify-content: center;
  padding: 8px 30px;
  margin-bottom: 0;
  border-radius: 12px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  opacity: 0.4;
  transform: scale(0.95);
  position: relative;
  cursor: pointer;
}

.lyric-line:hover {
  opacity: 0.7;
  transform: scale(0.98);
}

.lyric-line.passed {
  opacity: 0.5;
}

.lyric-line.active {
  opacity: 1;
  transform: scale(1.02);
}

.lyric-content {
  flex: 1;
  text-align: center;
  position: relative;
  display: inline-block;
  max-width: 100%;
}

.lyric-content .original {
  font-size: 22px;
  line-height: 1.5;
  color: #ccd6f6;
  margin-bottom: 4px;
  font-weight: 500;
  display: inline-block;
  padding-bottom: 0;
}

.lyric-line.active .original {
  color: #64ffda;
  font-weight: 600;
  font-size: 24px;
  text-shadow: 0 0 15px rgba(100, 255, 218, 0.3);
}

.lyric-content .translation {
  font-size: 16px;
  line-height: 1.4;
  color: #8892b0;
  margin-top: 2px;
}

.lyric-line.active .translation {
  color: #00d4ff;
  font-weight: 500;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .lyrics-scroll {
    padding: 30px 20px 80px;
  }

  .lyric-line {
    padding: 15px 20px;
  }

  .lyric-content .original {
    font-size: 18px;
  }

  .lyric-line.active .original {
    font-size: 20px;
  }
}
</style>
