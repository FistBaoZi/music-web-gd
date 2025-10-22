<template>
  <Transition name="slide-fade">
    <div class="lyrics-panel" v-if="showLyrics">
      <div class="panel-header">
        <h3>歌词</h3>
        <button @click="closeLyrics" class="close-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
      
      <div class="lyrics-content" ref="lyricsContainer">
        <div v-if="parsedLyrics.length === 0" class="no-lyrics">
          <i class="fas fa-music"></i>
          <p>暂无歌词</p>
        </div>
        <div v-else class="lyrics-lines">
          <div 
            v-for="(line, index) in parsedLyrics" 
            :key="index"
            class="lyric-line"
            :class="{ active: currentLyricIndex === index }"
            :ref="el => { if (el && currentLyricIndex === index) scrollToLyric(el) }"
          >
            <p class="original">{{ line.text }}</p>
            <p v-if="line.translation" class="translation">{{ line.translation }}</p>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useMusicStore } from '../stores/music'

const musicStore = useMusicStore()
const lyricsContainer = ref(null)
const currentLyricIndex = ref(0)
const currentTime = ref(0)

const parsedLyrics = computed(() => musicStore.parsedLyrics)
const showLyrics = computed(() => musicStore.showLyrics)

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
  if (el && lyricsContainer.value) {
    el.scrollIntoView({ behavior: 'smooth', block: 'center' })
  }
}

const closeLyrics = () => {
  musicStore.toggleLyrics()
}

onMounted(() => {
  window.addEventListener('lyric-time-update', handleTimeUpdate)
})

onUnmounted(() => {
  window.removeEventListener('lyric-time-update', handleTimeUpdate)
})
</script>

<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.3s ease-in;
}

.slide-fade-enter-from {
  transform: translateY(20px);
  opacity: 0;
}

.slide-fade-leave-to {
  transform: translateY(20px);
  opacity: 0;
}

.lyrics-panel {
  position: fixed;
  right: 20px;
  bottom: 120px;
  width: 400px;
  height: 500px;
  background: rgba(15, 12, 41, 0.98);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  box-shadow: 0 0 40px rgba(100, 255, 218, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.3);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  z-index: 1000;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid rgba(100, 255, 218, 0.2);
  background: rgba(30, 30, 50, 0.5);
}

.panel-header h3 {
  font-size: 18px;
  color: #ccd6f6;
  font-weight: 600;
}

.close-btn {
  width: 32px;
  height: 32px;
  border: none;
  background: rgba(100, 255, 218, 0.1);
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  color: #64ffda;
  border: 1px solid rgba(100, 255, 218, 0.3);
}

.close-btn:hover {
  background: rgba(100, 255, 218, 0.2);
  color: #64ffda;
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.4);
}

.lyrics-content {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
}

.lyrics-content::-webkit-scrollbar {
  width: 6px;
}

.lyrics-content::-webkit-scrollbar-thumb {
  background: rgba(100, 255, 218, 0.3);
  border-radius: 3px;
}

.lyrics-content::-webkit-scrollbar-thumb:hover {
  background: rgba(100, 255, 218, 0.5);
}

.no-lyrics {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #8892b0;
}

.no-lyrics i {
  font-size: 48px;
  margin-bottom: 15px;
  color: rgba(100, 255, 218, 0.3);
}

.lyrics-lines {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.lyric-line {
  padding: 10px;
  border-radius: 8px;
  transition: all 0.3s;
  opacity: 0.5;
}

.lyric-line.active {
  opacity: 1;
  background: linear-gradient(90deg, rgba(100, 255, 218, 0.15), transparent);
  transform: scale(1.05);
  border-left: 3px solid #64ffda;
  padding-left: 15px;
}

.lyric-line .original {
  font-size: 16px;
  color: #ccd6f6;
  line-height: 1.8;
  margin-bottom: 5px;
  font-weight: 500;
}

.lyric-line.active .original {
  color: #64ffda;
  font-weight: 600;
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.3);
}

.lyric-line .translation {
  font-size: 13px;
  color: #8892b0;
  line-height: 1.6;
}

.lyric-line.active .translation {
  color: #00d4ff;
}
</style>
