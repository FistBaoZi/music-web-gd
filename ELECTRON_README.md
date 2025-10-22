# Electron 桌面应用打包说明

## 开发模式

### 运行 Web 版本（浏览器）
```bash
npm run dev
```

### 运行 Electron 桌面版
```bash
npm run dev:electron
```

## 打包发布

### 打包 Windows 客户端
```bash
npm run build:win
```

打包完成后，安装包会生成在 `release` 目录中，文件名为 `在线音乐播放器-1.0.0-Setup.exe`

### 打包所有平台
```bash
npm run build:electron
```

## 注意事项

1. **应用图标**：请将应用图标放在 `build` 目录中：
   - Windows: `build/icon.ico` (256x256 或更大)
   - macOS: `build/icon.icns`
   - Linux: `build/icon.png` (512x512)

2. **首次打包**：首次打包可能需要下载一些依赖，请耐心等待

3. **打包配置**：打包相关配置在 `electron-builder.json` 文件中

## 项目结构

```
music-web-gd/
├── electron/              # Electron 主进程代码
│   ├── main.js           # 主进程入口
│   └── preload.js        # 预加载脚本
├── src/                  # Vue 应用源码
├── dist/                 # Web 构建输出
├── dist-electron/        # Electron 构建输出
├── release/              # 打包后的安装程序
├── electron-builder.json # Electron 打包配置
└── vite.config.js        # Vite 配置（已集成 Electron）
```

## 自定义配置

- **窗口大小**：修改 `electron/main.js` 中的 `width` 和 `height`
- **应用名称**：修改 `electron-builder.json` 中的 `productName`
- **应用 ID**：修改 `electron-builder.json` 中的 `appId`
