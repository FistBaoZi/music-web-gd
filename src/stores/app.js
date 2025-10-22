import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export const useAppStore = defineStore('app', () => {
  // 从 localStorage 加载当前视图
  const savedView = localStorage.getItem('currentView')
  const currentView = ref(savedView || 'discover')

  const setCurrentView = (view) => {
    currentView.value = view
  }

  // 监听并保存到 localStorage
  watch(currentView, (newValue) => {
    localStorage.setItem('currentView', newValue)
  })

  return {
    currentView,
    setCurrentView
  }
})
