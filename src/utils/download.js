// 下载工具函数

/**
 * 下载歌曲
 * @param {string} url - 歌曲URL
 * @param {string} filename - 文件名
 */
export const downloadFile = (url, filename) => {
  try {
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    link.target = '_blank'
    // 添加到DOM
    document.body.appendChild(link)
    // 触发下载
    link.click()
    // 移除元素
    document.body.removeChild(link)
    console.log(`正在下载: ${filename}`)
    return true
  } catch (error) {
    console.error('下载失败:', error)
    return false
  }
}

/**
 * 从歌曲对象生成文件名
 * @param {Object} song - 歌曲对象
 * @returns {string} 文件名
 */
export const generateFilename = (song) => {
  const artist = Array.isArray(song.artist) ? song.artist.join(', ') : song.artist || '未知歌手'
  const name = song.name || '未知歌曲'
  return `${name} - ${artist}.mp3`
}

/**
 * 下载歌曲（包含获取URL的逻辑）
 * @param {Object} song - 歌曲对象
 * @param {Function} getSongUrlFunc - 获取歌曲URL的函数
 * @returns {Promise<boolean>}
 */
export const downloadSong = async (song, getSongUrlFunc) => {
  try {
    // 如果歌曲已有URL，直接下载
    if (song.url) {
      const filename = generateFilename(song)
      return downloadFile(song.url, filename)
    }

    // 否则先获取URL
    if (!getSongUrlFunc) {
      alert('无法获取歌曲下载链接')
      return false
    }

    const urlData = await getSongUrlFunc(song.id, song.source || 'netease', 320)
    
    if (!urlData || !urlData.url) {
      alert('无法获取歌曲下载链接，可能是版权限制')
      return false
    }

    const filename = generateFilename(song)
    return downloadFile(urlData.url, filename)
  } catch (error) {
    console.error('下载失败:', error)
    alert('下载失败，请稍后重试')
    return false
  }
}
