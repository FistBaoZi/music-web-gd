// localStorage 工具函数

/**
 * 从 localStorage 加载数据
 */
export const loadFromStorage = (key, defaultValue) => {
  try {
    const saved = localStorage.getItem(key)
    return saved ? JSON.parse(saved) : defaultValue
  } catch (error) {
    console.error(`Failed to load ${key} from localStorage:`, error)
    return defaultValue
  }
}

/**
 * 保存数据到 localStorage
 */
export const saveToStorage = (key, value) => {
  try {
    localStorage.setItem(key, JSON.stringify(value))
  } catch (error) {
    console.error(`Failed to save ${key} to localStorage:`, error)
  }
}

/**
 * 从 localStorage 删除数据
 */
export const removeFromStorage = (key) => {
  try {
    localStorage.removeItem(key)
  } catch (error) {
    console.error(`Failed to remove ${key} from localStorage:`, error)
  }
}

/**
 * 清除所有音乐相关的缓存数据
 */
export const clearMusicCache = () => {
  const keys = [
    'currentSong',
    'playlist',
    'currentIndex',
    'showLyrics',
    'lyrics',
    'currentSource',
    'searchResults',
    'currentView',
    'lastSearchKeyword',
    'currentPlayTime',
    'volume',
    'playMode'
  ]
  
  keys.forEach(key => removeFromStorage(key))
  console.log('音乐缓存已清除')
}

/**
 * 获取缓存大小（估算）
 */
export const getCacheSize = () => {
  let total = 0
  for (let key in localStorage) {
    if (localStorage.hasOwnProperty(key)) {
      total += localStorage[key].length + key.length
    }
  }
  return (total / 1024).toFixed(2) + ' KB'
}
