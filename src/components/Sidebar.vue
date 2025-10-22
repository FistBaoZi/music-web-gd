<template>
  <aside class="sidebar">
    <nav>
      <div 
        class="nav-item" 
        :class="{ active: currentView === 'discover' }"
        @click="setView('discover')"
      >
        <i class="fas fa-compass"></i>
        <span>发现音乐</span>
      </div>
      <div 
        class="nav-item"
        :class="{ active: currentView === 'search' }"
        @click="setView('search')"
      >
        <i class="fas fa-search"></i>
        <span>搜索结果</span>
      </div>
      <div 
        class="nav-item"
        :class="{ active: currentView === 'lyrics' }"
        @click="setView('lyrics')"
      >
        <i class="fas fa-align-left"></i>
        <span>歌词展示</span>
      </div>
      <div 
        class="nav-item"
        :class="{ active: currentView === 'playlist' }"
        @click="setView('playlist')"
      >
        <i class="fas fa-list"></i>
        <span>播放列表</span>
      </div>
      <div 
        class="nav-item"
        :class="{ active: currentView === 'favorites' }"
        @click="setView('favorites')"
      >
        <i class="fas fa-heart"></i>
        <span>我的收藏</span>
      </div>
    </nav>
    <div class="sidebar-footer">
      <p>数据来源: GD音乐台</p>
      <p class="disclaimer">仅供学习使用</p>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useAppStore } from '../stores/app'

const appStore = useAppStore()
const currentView = computed(() => appStore.currentView)

const setView = (view) => {
  appStore.setCurrentView(view)
}
</script>

<style scoped>
.sidebar {
  width: 220px;
  background: rgba(15, 12, 41, 0.8);
  backdrop-filter: blur(10px);
  margin: 10px 0 10px 10px;
  border-radius: 15px;
  padding: 20px 0;
  display: flex;
  flex-direction: column;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
}

nav {
  flex: 1;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 15px 25px;
  color: #8892b0;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 16px;
}

.nav-item:hover {
  background: rgba(100, 255, 218, 0.1);
  color: #64ffda;
}

.nav-item.active {
  background: rgba(100, 255, 218, 0.15);
  font-weight: 600;
  color: #64ffda;
  box-shadow: 0 0 15px rgba(100, 255, 218, 0.2);
}

.nav-item i {
  font-size: 20px;
  width: 25px;
  text-align: center;
}

.sidebar-footer {
  padding: 20px;
  text-align: center;
  color: rgba(136, 146, 176, 0.8);
  font-size: 12px;
  border-top: 1px solid rgba(100, 255, 218, 0.1);
}

.disclaimer {
  margin-top: 5px;
  font-size: 11px;
  opacity: 0.6;
}
</style>
