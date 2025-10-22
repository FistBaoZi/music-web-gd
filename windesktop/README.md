# 音乐播放器 - .NET 8 桌面版

基于 .NET 8 + WebView2 的在线音乐播放器 Windows 桌面客户端。

## 📋 项目说明

这是使用 .NET 8 WPF 和 Microsoft Edge WebView2 开发的桌面应用，嵌入了 Vue 3 前端项目。

## ✨ 特性

- ✅ 基于 .NET 8 LTS 和 WPF
- ✅ 使用 WebView2 嵌入前端（基于 Chromium）
- ✅ 单文件发布，包含所有依赖
- ✅ 体积小（约 70-80 MB）
- ✅ 启动速度快，内存占用低
- ✅ 支持现代 Web 技术（Vue 3、ES6+）
- ✅ 稳定性优于 Electron

## 🛠️ 系统要求

### 开发环境
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js 16+ 和 npm（用于构建前端）
- Visual Studio 2022 或 VS Code（可选）

### 运行环境
- Windows 10/11 (x64)
- Microsoft Edge WebView2 Runtime（首次运行会自动下载）

## 🚀 快速开始

### 方法一：使用批处理脚本（推荐）

1. **构建项目**
   ```cmd
   cd windesktop
   build.bat
   ```

2. **运行程序**
   ```cmd
   run.bat
   ```

3. **发布单文件版本**
   ```cmd
   publish.bat
   ```
   发布的文件在 `windesktop\publish\` 目录

### 方法二：使用 dotnet 命令

1. **构建前端**
   ```cmd
   npm run build
   ```

2. **复制前端文件**
   ```cmd
   xcopy /E /I /Y dist windesktop\wwwroot
   ```

3. **构建 .NET 项目**
   ```cmd
   cd windesktop
   dotnet restore
   dotnet build -c Release
   ```

4. **运行**
   ```cmd
   dotnet run
   ```

5. **发布（单文件自包含）**
   ```cmd
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
   ```

## 📁 项目结构

```
windesktop/
├── App.xaml                    # WPF 应用程序入口
├── App.xaml.cs                 # 应用程序逻辑
├── MainWindow.xaml             # 主窗口界面
├── MainWindow.xaml.cs          # 主窗口逻辑（WebView2 初始化）
├── MusicPlayer.csproj          # 项目文件
├── build.bat                   # 构建脚本
├── publish.bat                 # 发布脚本
├── run.bat                     # 运行脚本
├── wwwroot/                    # 前端构建文件（从 ../dist 复制）
│   ├── index.html
│   ├── assets/
│   └── ...
└── README.md                   # 本文件
```

## 🔧 配置说明

### WebView2 配置

在 `MainWindow.xaml.cs` 中可以配置：

- **开发工具**：`AreDevToolsEnabled = true`（F12 打开）
- **右键菜单**：`AreDefaultContextMenusEnabled = true`
- **缩放控制**：`IsZoomControlEnabled = false`
- **用户数据文件夹**：`%LocalAppData%\MusicPlayer\WebView2`

### 虚拟主机名

使用虚拟主机名 `https://app.local` 映射本地文件夹，避免 `file://` 协议限制。

```csharp
webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
    "app.local",
    distPath,
    CoreWebView2HostResourceAccessKind.Allow
);
```

## 🎯 与 Electron 对比

| 特性 | .NET 8 + WebView2 | Electron |
|------|-------------------|----------|
| 文件大小 | ~70-80 MB | ~150 MB+ |
| 启动速度 | 快 | 较慢 |
| 内存占用 | 较低 | 较高 |
| 跨平台 | ❌ Windows Only | ✅ Win/Mac/Linux |
| 开发语言 | C# | JavaScript |
| 运行时依赖 | 系统 WebView2 | 内置 Chromium |
| 稳定性 | 高（LTS） | 中 |

## 📝 开发说明

### 前后端通信

**前端发送消息到 C#：**
```javascript
window.chrome.webview.postMessage('Hello from Vue!');
```

**C# 接收消息：**
```csharp
webView.CoreWebView2.WebMessageReceived += (sender, e) => {
    var message = e.TryGetWebMessageAsString();
    // 处理消息
};
```

**C# 发送消息到前端：**
```csharp
webView.CoreWebView2.PostWebMessageAsString("Hello from C#!");
```

**前端接收消息：**
```javascript
window.chrome.webview.addEventListener('message', (e) => {
    console.log('Message from C#:', e.data);
});
```

### 扩展功能建议

- ✅ 系统托盘集成
- ✅ 全局快捷键
- ✅ 媒体控制（键盘多媒体键）
- ✅ 文件下载管理
- ✅ 自动更新
- ✅ 主题跟随系统

## ❓ 常见问题

### 1. 提示未找到 WebView2 Runtime

首次运行时会自动下载，或手动下载：
https://developer.microsoft.com/microsoft-edge/webview2/

### 2. 提示未找到前端文件

确保先运行 `build.bat` 或手动构建前端并复制到 `wwwroot` 文件夹。

### 3. 发布后文件太大

使用单文件发布会包含 .NET 运行时。如果目标机器已安装 .NET 8，可以使用依赖框架的发布：
```cmd
dotnet publish -c Release -r win-x64 --self-contained false
```

### 4. 如何调试前端

按 F12 打开开发者工具（需要 `AreDevToolsEnabled = true`）

## 📦 发布清单

发布时需要的文件：
- ✅ `MusicPlayer.exe`（主程序）
- ✅ WebView2 Runtime（自动检测并下载）

不需要的文件（已嵌入）：
- ❌ `wwwroot/` 文件夹
- ❌ `.NET` 运行时
- ❌ 其他 DLL 文件

## 📄 许可证

与主项目相同

## 👨‍💻 作者

FistBaoZi

---

**提示**：开发时建议使用 Visual Studio 2022 打开 `MusicPlayer.csproj` 进行调试。
