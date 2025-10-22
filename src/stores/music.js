import { defineStore } from 'pinia'
import { ref, computed, watch } from 'vue'
import * as musicApi from '../api/music'

// 从 localStorage 加载数据
const loadFromStorage = (key, defaultValue) => {
  try {
    const saved = localStorage.getItem(key)
    return saved ? JSON.parse(saved) : defaultValue
  } catch (error) {
    console.error(`Failed to load ${key} from localStorage:`, error)
    return defaultValue
  }
}

// 保存数据到 localStorage
const saveToStorage = (key, value) => {
  try {
    localStorage.setItem(key, JSON.stringify(value))
  } catch (error) {
    console.error(`Failed to save ${key} to localStorage:`, error)
  }
}

export const useMusicStore = defineStore('music', () => {
  const searchResults = ref(loadFromStorage('searchResults', []))
  const currentSong = ref(loadFromStorage('currentSong', null))
  const playlist = ref(loadFromStorage('playlist', []))
  const currentIndex = ref(loadFromStorage('currentIndex', 0))
  const showLyrics = ref(loadFromStorage('showLyrics', false))
  const lyrics = ref(loadFromStorage('lyrics', { lyric: '', tlyric: '' }))
  const loading = ref(false) // 播放加载状态（保留用于兼容）
  const searchLoading = ref(false) // 搜索加载状态
  const currentSource = ref(loadFromStorage('currentSource', 'netease'))

  // 监听并保存到 localStorage
  watch(searchResults, (newValue) => {
    saveToStorage('searchResults', newValue)
  }, { deep: true })

  watch(currentSong, (newValue) => {
    saveToStorage('currentSong', newValue)
  }, { deep: true })

  watch(playlist, (newValue) => {
    saveToStorage('playlist', newValue)
  }, { deep: true })

  watch(currentIndex, (newValue) => {
    saveToStorage('currentIndex', newValue)
  })

  watch(showLyrics, (newValue) => {
    saveToStorage('showLyrics', newValue)
  })

  watch(lyrics, (newValue) => {
    saveToStorage('lyrics', newValue)
  }, { deep: true })

  watch(currentSource, (newValue) => {
    saveToStorage('currentSource', newValue)
  })

  // 搜索音乐
  const searchMusic = async (keyword, source = 'netease') => {
    searchLoading.value = true
    currentSource.value = source
    try {
      const data = await musicApi.searchMusic(keyword, source, 30, 1)
      searchResults.value = data || []
    } catch (error) {
      console.error('搜索失败:', error)
      searchResults.value = []
    } finally {
      searchLoading.value = false
    }
  }

  // 播放歌曲
  const playSong = async (song) => {
    loading.value = true
    try {
      // 获取歌曲URL
      const urlData = await musicApi.getSongUrl(song.id, song.source || currentSource.value)
      
      // 获取专辑图
      let picUrl = ''
      if (song.pic_id) {
        const picData = await musicApi.getAlbumPic(song.pic_id, song.source || currentSource.value)
        picUrl = picData.url
      }

      // 获取歌词
      const lyricData = await musicApi.getLyric(song.lyric_id || song.id, song.source || currentSource.value)
      lyrics.value = {
        lyric: lyricData.lyric || '',
        tlyric: lyricData.tlyric || ''
      }

      currentSong.value = {
        ...song,
        url: urlData.url,
        pic: picUrl,
        br: urlData.br,
        size: urlData.size
      }

      // 添加到播放列表
      if (!playlist.value.find(item => item.id === song.id && item.source === song.source)) {
        playlist.value.push(currentSong.value)
        currentIndex.value = playlist.value.length - 1
      } else {
        currentIndex.value = playlist.value.findIndex(
          item => item.id === song.id && item.source === song.source
        )
      }
    } catch (error) {
      console.error('播放歌曲失败:', error)
    } finally {
      loading.value = false
    }
  }

  // 播放下一曲
  const playNext = () => {
    if (playlist.value.length === 0) return
    currentIndex.value = (currentIndex.value + 1) % playlist.value.length
    playSong(playlist.value[currentIndex.value])
  }

  // 播放上一曲
  const playPrevious = () => {
    if (playlist.value.length === 0) return
    currentIndex.value = currentIndex.value === 0 
      ? playlist.value.length - 1 
      : currentIndex.value - 1
    playSong(playlist.value[currentIndex.value])
  }

  // 随机播放
  const playRandom = () => {
    if (playlist.value.length === 0) return
    const randomIndex = Math.floor(Math.random() * playlist.value.length)
    currentIndex.value = randomIndex
    playSong(playlist.value[randomIndex])
  }

  // 移除播放列表中的歌曲
  const removeFromPlaylist = (index) => {
    playlist.value.splice(index, 1)
    if (currentIndex.value >= index && currentIndex.value > 0) {
      currentIndex.value--
    }
  }

  // 清空播放列表
  const clearPlaylist = () => {
    playlist.value = []
    currentIndex.value = 0
    currentSong.value = null
  }

  // 切换歌词显示
  const toggleLyrics = () => {
    showLyrics.value = !showLyrics.value
  }

  // 解析歌词
  const parsedLyrics = computed(() => {
    const parseLrc = (lrcText) => {
      if (!lrcText) return []
      const lines = lrcText.split('\n')
      const result = []
      
      lines.forEach(line => {
        const match = line.match(/\[(\d{2}):(\d{2})\.(\d{2,3})\](.*)/)
        if (match) {
          const minutes = parseInt(match[1])
          const seconds = parseInt(match[2])
          const milliseconds = parseInt(match[3])
          const time = minutes * 60 + seconds + milliseconds / 1000
          const text = match[4].trim()
          if (text) {
            result.push({ time, text })
          }
        }
      })
      
      return result
    }

    const lyricLines = parseLrc(lyrics.value.lyric)
    const tlyricLines = parseLrc(lyrics.value.tlyric)
    
    // 合并原文和翻译
    return lyricLines.map(line => {
      const translation = tlyricLines.find(t => Math.abs(t.time - line.time) < 0.5)
      return {
        time: line.time,
        text: line.text,
        translation: translation?.text || ''
      }
    })
  })

  return {
    searchResults,
    currentSong,
    playlist,
    currentIndex,
    showLyrics,
    lyrics,
    parsedLyrics,
    loading,
    searchLoading,
    currentSource,
    searchMusic,
    playSong,
    playNext,
    playPrevious,
    playRandom,
    removeFromPlaylist,
    clearPlaylist,
    toggleLyrics
  }
})
