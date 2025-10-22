@echo off
chcp 65001 >nul
REM ========================================
REM 音乐播放器 - 发布脚本
REM ========================================

echo ======================================
echo 开始发布音乐播放器...
echo ======================================
echo.

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

REM 4. 发布单文件自包含应用（.NET 8 稳定版本）
echo [4/5] 发布 .NET 桌面应用（单文件版本）...
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=false -o publish
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 发布失败！
    pause
    exit /b 1
)
echo ✓ 发布完成
echo.

REM 5. 创建版本信息文件
echo [5/5] 创建版本信息...
echo 音乐播放器 v1.0.0 > publish\VERSION.txt
echo 构建时间: %date% %time% >> publish\VERSION.txt
echo 运行要求: Windows 10/11 (x64) >> publish\VERSION.txt
echo .NET 版本: .NET 8.0 >> publish\VERSION.txt
echo. >> publish\VERSION.txt
echo 说明: >> publish\VERSION.txt
echo - 双击 MusicPlayer.exe 启动程序 >> publish\VERSION.txt
echo - 首次运行会自动下载 WebView2 Runtime（如果未安装） >> publish\VERSION.txt
echo - 单文件版本，包含所有依赖 >> publish\VERSION.txt
echo ✓ 版本信息创建完成
echo.

echo ======================================
echo 发布完成！
echo.
echo 发布文件位置：windesktop\publish\
echo 主程序：MusicPlayer.exe
echo.
echo 文件大小约：70-80 MB（单文件，包含所有依赖）
echo 可以直接分发 MusicPlayer.exe，无需其他文件
echo ======================================
pause
