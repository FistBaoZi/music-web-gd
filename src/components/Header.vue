<template>
  <header class="header">
    <div class="logo">
      <i class="fas fa-music"></i>
      <span>GD 音乐台</span>
    </div>
    <div class="search-bar">
      <input 
        type="text" 
        v-model="searchKeyword"
        @keyup.enter="handleSearch"
        placeholder="搜索音乐、歌手、专辑..."
      >
      <button @click="handleSearch">
        <i class="fas fa-search"></i>
      </button>
    </div>
    <div class="source-selector">
      <select v-model="musicSource">
        <option value="netease">网易云</option>
        <option value="kuwo">酷我</option>
        <option value="joox">JOOX</option>
        <option value="tencent">QQ音乐</option>
        <option value="kugou">酷狗</option>
      </select>
    </div>
  </header>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useAppStore } from '../stores/app'
import { useMusicStore } from '../stores/music'

const appStore = useAppStore()
const musicStore = useMusicStore()

const searchKeyword = ref(localStorage.getItem('lastSearchKeyword') || '')
const musicSource = ref(musicStore.currentSource)

// 监听音乐源变化，同步到 store
watch(musicSource, (newValue) => {
  musicStore.currentSource = newValue
})

const handleSearch = () => {
  if (searchKeyword.value.trim()) {
    musicStore.searchMusic(searchKeyword.value, musicSource.value)
    appStore.setCurrentView('search')
    // 保存搜索关键词
    localStorage.setItem('lastSearchKeyword', searchKeyword.value)
  }
}
</script>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 15px 30px;
  background: rgba(15, 12, 41, 0.8);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid rgba(100, 255, 218, 0.2);
}

.logo {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 24px;
  font-weight: bold;
  color: #64ffda;
  text-shadow: 0 0 10px rgba(100, 255, 218, 0.5);
}

.logo i {
  font-size: 28px;
}

.search-bar {
  display: flex;
  flex: 1;
  max-width: 500px;
  margin: 0 30px;
  background: white;
  border-radius: 25px;
  overflow: hidden;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.search-bar input {
  flex: 1;
  padding: 12px 20px;
  border: none;
  outline: none;
  font-size: 14px;
}

.search-bar button {
  padding: 0 20px;
  border: none;
  background: linear-gradient(135deg, #64ffda 0%, #00d4ff 100%);
  color: #0f0c29;
  cursor: pointer;
  transition: all 0.3s;
  font-weight: 600;
}

.search-bar button:hover {
  background: linear-gradient(135deg, #00d4ff 0%, #64ffda 100%);
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.5);
}

.source-selector select {
  padding: 10px 15px;
  border: 2px solid #64ffda;
  background: rgba(15, 12, 41, 0.6);
  color: #64ffda;
  border-radius: 20px;
  outline: none;
  cursor: pointer;
  font-weight: 500;
}

.source-selector select option {
  background: #0f0c29;
  color: #64ffda;
}
</style>
