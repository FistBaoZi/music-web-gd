<template>
  <div class="app">
    <ParticleBackground />
    <Header />
    <div class="main-container">
      <Sidebar />
      <div class="content">
        <SearchView v-if="currentView === 'search'" />
        <PlaylistView v-else-if="currentView === 'playlist'" />
        <LyricsView v-else-if="currentView === 'lyrics'" />
        <DiscoverView v-else />
      </div>
    </div>
    <Player />
    <LyricsPanel />
    <ClickEffect />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import ParticleBackground from './components/ParticleBackground.vue'
import Header from './components/Header.vue'
import Sidebar from './components/Sidebar.vue'
import Player from './components/Player.vue'
import LyricsPanel from './components/LyricsPanel.vue'
import ClickEffect from './components/ClickEffect.vue'
import SearchView from './views/SearchView.vue'
import PlaylistView from './views/PlaylistView.vue'
import LyricsView from './views/LyricsView.vue'
import DiscoverView from './views/DiscoverView.vue'
import { useAppStore } from './stores/app'

const appStore = useAppStore()
const currentView = computed(() => appStore.currentView)
</script>

<style scoped>
.app {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  position: relative;
}

.main-container {
  display: flex;
  flex: 1;
  overflow: hidden;
  position: relative;
  z-index: 1;
  gap: 10px;
  padding: 0 10px;
}

.content {
  flex: 1;
  overflow-y: auto;
  background: rgba(20, 20, 35, 0.95);
  margin: 10px 0;
  border-radius: 15px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
}
</style>
