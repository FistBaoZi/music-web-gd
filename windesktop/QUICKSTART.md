# 快速入门指南

## 🚀 5 分钟快速启动

### 前置条件检查

1. **安装 .NET 8 SDK**
   ```cmd
   dotnet --version
   ```
   如果显示版本号（8.x.x），说明已安装。否则从 https://dotnet.microsoft.com/download/dotnet/8.0 下载安装。

2. **确认 Node.js 已安装**
   ```cmd
   node --version
   npm --version
   ```

### 第一次运行

1. **打开命令提示符（CMD）或 PowerShell**

2. **进入 windesktop 目录**
   ```cmd
   cd windesktop
   ```

3. **运行构建脚本**
   ```cmd
   build.bat
   ```
   这个脚本会：
   - ✅ 构建 Vue 前端
   - ✅ 复制文件到 wwwroot
   - ✅ 还原 .NET 依赖
   - ✅ 构建桌面程序

4. **运行程序**
   ```cmd
   run.bat
   ```

### 开发调试

**使用 Visual Studio 2022（推荐）：**
1. 双击打开 `MusicPlayer.sln`
2. 按 F5 启动调试

**使用 VS Code：**
1. 打开 windesktop 文件夹
2. 按 F5 启动调试（需要安装 C# 扩展）

**使用命令行：**
```cmd
dotnet run
```

### 发布可执行文件

```cmd
publish.bat
```

发布的单文件版本在 `publish\MusicPlayer.exe`，可以直接分发给用户。

## ⚠️ 常见错误处理

### 错误：未找到 dotnet 命令
**解决**：安装 .NET 8 SDK

### 错误：未找到前端文件
**解决**：先回到根目录运行 `npm run build`

### 错误：WebView2 Runtime 未安装
**解决**：程序会提示下载，或访问 https://go.microsoft.com/fwlink/p/?LinkId=2124703

### 错误：图标文件缺失
**解决**：参考 `ICON_README.md` 添加图标，或临时注释掉图标引用

## 📚 更多信息

详细文档请查看 `README.md`
