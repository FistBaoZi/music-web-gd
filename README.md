# 在线音乐播放器

一个基于 Vue3 + Vite 构建的现代化在线音乐播放器，使用 GD Studio 音乐 API。

## 功能特性

- 🎵 **多平台搜索**：支持网易云、QQ音乐、酷我、酷狗等多个音乐平台
- 🎧 **高品质音乐**：支持最高无损音质播放 (320kbps/FLAC)
- 📝 **实时歌词**：支持 LRC 格式歌词显示，带中文翻译
- 📋 **播放列表**：自定义播放列表管理
- 💾 **数据持久化**：刷新页面自动保存播放状态和进度
- 📥 **歌曲下载**：支持下载当前播放或列表中的歌曲
- 🎨 **现代化UI**：渐变色设计，流畅的交互动画
- 📱 **响应式设计**：适配桌面端和移动端

## 技术栈

- **前端框架**：Vue 3 (Composition API)
- **构建工具**：Vite
- **状态管理**：Pinia
- **HTTP 客户端**：Axios
- **图标库**：Font Awesome 6

## 项目结构

```
music/
├── src/
│   ├── api/              # API 接口封装
│   │   └── music.js      # 音乐 API
│   ├── assets/           # 静态资源
│   │   └── styles/       # 样式文件
│   │       └── main.css  # 全局样式
│   ├── components/       # 组件
│   │   ├── Header.vue    # 顶部导航栏
│   │   ├── Sidebar.vue   # 侧边栏
│   │   ├── Player.vue    # 底部播放器
│   │   └── LyricsPanel.vue # 歌词面板
│   ├── stores/           # 状态管理
│   │   ├── app.js        # 应用状态
│   │   └── music.js      # 音乐状态
│   ├── views/            # 页面视图
│   │   ├── DiscoverView.vue  # 发现页
│   │   ├── SearchView.vue    # 搜索结果页
│   │   └── PlaylistView.vue  # 播放列表页
│   ├── App.vue           # 根组件
│   └── main.js           # 入口文件
├── index.html            # HTML 模板
├── vite.config.js        # Vite 配置
└── package.json          # 项目依赖

```

## 快速开始

### 安装依赖

```bash
npm install
```

### 启动开发服务器

```bash
npm run dev
```

访问 http://localhost:3000

### 构建生产版本

```bash
npm run build
```

### 预览生产构建

```bash
npm run preview
```

## API 说明

本项目使用 GD Studio 音乐 API：

- **API 地址**：https://music-api.gdstudio.xyz/api.php
- **数据来源**：music.gdstudio.xyz

### 主要接口

1. **搜索音乐**
   - 参数：`types=search&source=netease&name=关键字&count=20&pages=1`
   
2. **获取歌曲 URL**
   - 参数：`types=url&source=netease&id=歌曲ID&br=320`
   
3. **获取专辑图**
   - 参数：`types=pic&source=netease&id=专辑图ID&size=500`
   
4. **获取歌词**
   - 参数：`types=lyric&source=netease&id=歌词ID`

### 支持的音乐源

- ✅ netease (网易云音乐) - 推荐
- ✅ kuwo (酷我音乐) - 推荐
- ✅ joox (JOOX) - 推荐
- tencent (QQ音乐)
- kugou (酷狗音乐)
- migu (咪咕音乐)
- 其他平台...

## 使用说明

1. **搜索音乐**
   - 在顶部搜索框输入歌曲名、歌手或专辑
   - 选择音乐源（建议使用网易云、酷我、JOOX）
   - 按回车或点击搜索按钮

2. **播放音乐**
   - 双击歌曲直接播放
   - 单击播放按钮添加到播放列表
   - 使用播放器控制播放/暂停、上/下一曲

3. **下载歌曲**
   - 搜索结果页：点击歌曲右侧的下载按钮
   - 播放列表页：点击歌曲右侧的下载按钮
   - 歌词页面：点击"下载歌曲"按钮
   - 播放器：点击下载图标下载当前播放歌曲

4. **查看歌词**
   - 点击播放器上的歌词按钮显示歌词面板
   - 点击侧边栏"歌词展示"查看全屏歌词
   - 歌词自动滚动并高亮当前行

5. **播放模式**
   - 列表循环：播放完列表后重新开始
   - 随机播放：随机选择列表中的歌曲
   - 单曲循环：重复播放当前歌曲

## 注意事项

⚠️ **免责声明**
- 本项目仅供学习和研究使用
- 请勿用于商业用途
- 音乐资源版权归原作者所有
- 请支持正版音乐

## 桌面客户端

本项目提供 Windows 桌面客户端版本：

### 🪟 .NET 8 桌面版（推荐）
- 📁 位置：`windesktop/` 目录
- ⚡ 基于 .NET 8 + WebView2
- 💪 性能优秀，体积小（约 70-80 MB 单文件）
- 📖 查看详情：[windesktop/README.md](windesktop/README.md)
- 🚀 快速开始：[windesktop/QUICKSTART.md](windesktop/QUICKSTART.md)

**快速构建：**
```bash
cd windesktop
build.bat        # 开发构建
publish.bat      # 发布单文件版本
```

## 开发计划

- [x] 添加歌词页面
- [x] 数据持久化功能
- [x] 歌曲下载功能
- [x] .NET 8 桌面客户端
- [ ] 添加收藏功能
- [ ] 支持歌单导入
- [ ] 优化移动端体验
- [ ] 添加音效均衡器
- [ ] 支持本地音乐播放
- [ ] 批量下载功能

## License

MIT License

## 致谢

- API 提供：[GD Studio](https://music.gdstudio.xyz)
- 基于开源项目：Meting & MKOnlineMusicPlayer
- 图标：Font Awesome

---

**数据来源**: GD音乐台 (music.gdstudio.xyz)

**仅供学习使用，请勿商用**
