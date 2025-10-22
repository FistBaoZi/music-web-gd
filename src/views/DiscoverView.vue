<template>
  <div class="discover-view">
    <ParticleBackground />
    <div class="quick-search">
      <h3>热门搜索</h3>
      <div class="tags">
        <span 
          v-for="tag in hotTags" 
          :key="tag"
          @click="searchTag(tag)"
          class="tag"
        >
          {{ tag }}
        </span>
      </div>
    </div>

    <div class="tips">
      <h3>使用提示</h3>
      <ul>
        <li>在顶部搜索框输入歌曲名、歌手或专辑进行搜索</li>
        <li>双击歌曲即可播放，单击播放按钮添加到播放列表</li>
        <li>支持多种音乐源切换，建议使用网易云、酷我、JOOX</li>
        <li>点击侧边栏"歌词展示"查看全屏歌词，支持实时滚动高亮</li>
        <li>播放器上的歌词按钮可显示/隐藏浮动歌词面板</li>
        <li>刷新页面会自动保存播放状态、列表和进度</li>
        <li>API文档地址：<a href="https://music-api.gdstudio.xyz/api.php" target="_blank" class="api-link">https://music-api.gdstudio.xyz/api.php</a></li>
        <li>本站资源仅供学习参考，请勿用于商业用途</li>
      </ul>
      <div class="cache-actions">
        <button @click="showCacheInfo" class="cache-btn">
          <i class="fas fa-info-circle"></i> 缓存信息
        </button>
        <button @click="clearCache" class="cache-btn clear">
          <i class="fas fa-trash"></i> 清除缓存
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAppStore } from '../stores/app'
import { useMusicStore } from '../stores/music'
import { clearMusicCache, getCacheSize } from '../utils/storage'
import ParticleBackground from '../components/ParticleBackground.vue'

const appStore = useAppStore()
const musicStore = useMusicStore()

const hotTags = ref([
  '周杰伦', '林俊杰', '陈奕迅', '邓紫棋',
  '薛之谦', '毛不易', '五月天', '许嵩',
  '热门歌曲', '流行音乐', '经典老歌', '粤语歌曲'
])

const searchTag = (tag) => {
  // 设置搜索关键词，Header 组件会自动响应并执行搜索
  appStore.setSearchKeyword(tag)
}

const showCacheInfo = () => {
  const size = getCacheSize()
  const info = `当前缓存大小: ${size}\n\n包含:\n- 播放列表\n- 当前播放歌曲\n- 播放进度\n- 搜索历史\n- 用户设置`
  alert(info)
}

const clearCache = () => {
  if (confirm('确定要清除所有缓存数据吗？\n这将清除播放列表、播放进度和搜索历史。')) {
    clearMusicCache()
    alert('缓存已清除！刷新页面后生效。')
    setTimeout(() => {
      window.location.reload()
    }, 1000)
  }
}
</script>

<style scoped>
.discover-view {
  padding: 30px;
  position: relative;
  min-height: 100%;
}

.quick-search {
  margin-bottom: 40px;
  position: relative;
  z-index: 1;
}

.quick-search h3 {
  font-size: 20px;
  color: #ccd6f6;
  margin-bottom: 20px;
}

.tags {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.tag {
  padding: 10px 20px;
  background: rgba(30, 30, 50, 0.4);
  border: 2px solid rgba(100, 255, 218, 0.3);
  color: #64ffda;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 14px;
  font-weight: 500;
}

.tag:hover {
  background: rgba(100, 255, 218, 0.2);
  border-color: #64ffda;
  color: #64ffda;
  transform: translateY(-2px);
  box-shadow: 0 0 20px rgba(100, 255, 218, 0.4);
}

.tips {
  background: rgba(30, 30, 50, 0.4);
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(100, 255, 218, 0.1);
}

.tips h3 {
  font-size: 20px;
  color: #ccd6f6;
  margin-bottom: 20px;
}

.tips ul {
  list-style: none;
  padding: 0;
}

.tips li {
  padding: 12px 0;
  padding-left: 30px;
  color: #8892b0;
  line-height: 1.8;
  position: relative;
}

.tips li::before {
  content: '•';
  position: absolute;
  left: 10px;
  color: #64ffda;
  font-size: 20px;
}

.api-link {
  color: #64ffda;
  text-decoration: underline;
  font-weight: 500;
  transition: color 0.3s;
}

.api-link:hover {
  color: #00d4ff;
}

.cache-actions {
  margin-top: 25px;
  padding-top: 20px;
  border-top: 1px solid rgba(100, 255, 218, 0.1);
  display: flex;
  gap: 15px;
  justify-content: center;
}

.cache-btn {
  padding: 10px 20px;
  border: 2px solid rgba(100, 255, 218, 0.3);
  background: rgba(30, 30, 50, 0.4);
  color: #64ffda;
  border-radius: 20px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.cache-btn:hover {
  background: rgba(100, 255, 218, 0.2);
  border-color: #64ffda;
  color: #64ffda;
  transform: translateY(-2px);
  box-shadow: 0 0 20px rgba(100, 255, 218, 0.4);
}

.cache-btn.clear {
  border-color: #ff4757;
  color: #ff4757;
}

.cache-btn.clear:hover {
  background: #ff4757;
  color: white;
  box-shadow: 0 4px 12px rgba(255, 71, 87, 0.3);
}
</style>
