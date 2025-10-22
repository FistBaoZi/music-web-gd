import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import electron from 'vite-plugin-electron/simple'

export default defineConfig(({ mode }) => {
  const isElectron = mode === 'electron'

  return {
    plugins: [
      vue(),
      isElectron && electron({
        main: {
          entry: 'electron/main.js',
        },
        preload: {
          input: 'electron/preload.js',
        },
      }),
    ].filter(Boolean),
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: {
      port: 3000,
      open: !isElectron
    },
    base: './', // 重要：使用相对路径，确保 Electron 可以正确加载资源
  }
})
