@echo off
chcp 65001 >nul
REM ========================================
REM 音乐播放器 - 单文件发布脚本（实验性）
REM 注意：WPF 单文件发布可能存在兼容性问题
REM 推荐使用 publish.bat 进行标准发布
REM ========================================

echo ======================================
echo 开始发布音乐播放器（单文件版本）...
echo ======================================
echo.
echo 警告：单文件发布可能在某些情况下出现 XAML 加载问题
echo 如果遇到问题，请使用 publish.bat 进行标准发布
echo.
pause

REM 1. 构建前端应用
echo [1/5] 构建 Vue 前端应用...
cd ..
call npm run build
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 前端构建失败！
    pause
    exit /b 1
)
echo ✓ 前端构建完成
echo.

REM 2. 复制前端文件到 WPF 项目
echo [2/5] 复制前端文件到 WPF 项目...
if exist "windesktop\wwwroot" (
    rmdir /s /q "windesktop\wwwroot"
)
xcopy /E /I /Y "dist" "windesktop\wwwroot"
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 复制文件失败！
    pause
    exit /b 1
)
echo ✓ 文件复制完成
echo.

REM 3. 还原 .NET 依赖
echo [3/5] 还原 .NET 依赖...
cd windesktop
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 还原依赖失败！
    pause
    exit /b 1
)
echo ✓ 依赖还原完成
echo.

REM 4. 发布单文件（使用 ReadyToRun 优化）
echo [4/5] 发布 .NET 桌面应用（单文件 + AOT）...
dotnet publish -c Release -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:PublishReadyToRun=true ^
    -p:PublishTrimmed=false ^
    -p:IncludeAllContentForSelfExtract=true ^
    -p:DebugType=embedded ^
    -o publish-singlefile
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 发布失败！
    echo 建议使用 publish.bat 进行标准发布
    pause
    exit /b 1
)
echo ✓ 发布完成
echo.

REM 5. 创建版本信息文件
echo [5/5] 创建版本信息...
echo 音乐播放器 v1.0.0 (单文件版本) > publish-singlefile\VERSION.txt
echo 构建时间: %date% %time% >> publish-singlefile\VERSION.txt
echo 运行要求: Windows 10/11 (x64) >> publish-singlefile\VERSION.txt
echo. >> publish-singlefile\VERSION.txt
echo 说明: >> publish-singlefile\VERSION.txt
echo - 双击 MusicPlayer.exe 启动程序 >> publish-singlefile\VERSION.txt
echo - 首次运行会自动下载 WebView2 Runtime（如果未安装） >> publish-singlefile\VERSION.txt
echo - 如果遇到启动问题，请使用标准版本（publish.bat） >> publish-singlefile\VERSION.txt
echo ✓ 版本信息创建完成
echo.

echo ======================================
echo 发布完成！
echo.
echo 发布文件位置：windesktop\publish-singlefile\
echo 主程序：MusicPlayer.exe
echo.
echo 文件大小约：80-100 MB（单个可执行文件）
echo.
echo 如果程序无法启动，请使用 publish.bat 生成标准版本
echo ======================================
pause
