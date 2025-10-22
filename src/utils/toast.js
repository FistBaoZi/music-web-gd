import { createApp } from 'vue'
import Toast from '../components/Toast.vue'

let toastInstance = null

const showToast = (message, type = 'success', duration = 3000) => {
  // 如果已有实例，先移除
  if (toastInstance) {
    toastInstance.unmount()
    toastInstance = null
  }

  // 创建容器
  const container = document.createElement('div')
  document.body.appendChild(container)

  // 创建 Toast 实例
  const app = createApp(Toast, {
    message,
    type,
    duration
  })

  toastInstance = app.mount(container)
  toastInstance.show()

  // 自动清理
  setTimeout(() => {
    if (toastInstance) {
      app.unmount()
      document.body.removeChild(container)
      toastInstance = null
    }
  }, duration + 500)
}

export const toast = {
  success: (message, duration) => showToast(message, 'success', duration),
  info: (message, duration) => showToast(message, 'info', duration),
  warning: (message, duration) => showToast(message, 'warning', duration),
  error: (message, duration) => showToast(message, 'error', duration)
}

export default toast
