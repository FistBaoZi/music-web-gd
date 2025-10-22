import { contextBridge, ipcRenderer } from 'electron'

// 暴露安全的 API 给渲染进程
contextBridge.exposeInMainWorld('electronAPI', {
  // 发送消息到主进程
  sendMessage: (message) => {
    ipcRenderer.send('message-from-renderer', message)
  },
  
  // 接收来自主进程的消息
  onMessage: (callback) => {
    ipcRenderer.on('message-from-main', (event, data) => {
      callback(data)
    })
  },

  // 获取平台信息
  platform: process.platform,
  
  // 获取应用版本
  getVersion: () => process.versions.electron
})
