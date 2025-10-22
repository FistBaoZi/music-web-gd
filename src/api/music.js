import axios from 'axios'

const API_BASE_URL = 'https://music-api.gdstudio.xyz/api.php'

// 搜索音乐
export const searchMusic = async (name, source = 'netease', count = 20, pages = 1) => {
  try {
    const response = await axios.get(API_BASE_URL, {
      params: {
        types: 'search',
        source,
        name,
        count,
        pages
      }
    })
    return response.data
  } catch (error) {
    console.error('搜索音乐失败:', error)
    throw error
  }
}

// 获取歌曲URL
export const getSongUrl = async (id, source = 'netease', br = 320) => {
  try {
    const response = await axios.get(API_BASE_URL, {
      params: {
        types: 'url',
        source,
        id,
        br
      }
    })
    return response.data
  } catch (error) {
    console.error('获取歌曲URL失败:', error)
    throw error
  }
}

// 获取专辑图
export const getAlbumPic = async (id, source = 'netease', size = 500) => {
  try {
    const response = await axios.get(API_BASE_URL, {
      params: {
        types: 'pic',
        source,
        id,
        size
      }
    })
    return response.data
  } catch (error) {
    console.error('获取专辑图失败:', error)
    throw error
  }
}

// 获取歌词
export const getLyric = async (id, source = 'netease') => {
  try {
    const response = await axios.get(API_BASE_URL, {
      params: {
        types: 'lyric',
        source,
        id
      }
    })
    return response.data
  } catch (error) {
    console.error('获取歌词失败:', error)
    throw error
  }
}
