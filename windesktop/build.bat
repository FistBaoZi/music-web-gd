@echo off
chcp 65001 >nul
REM ========================================
REM 音乐播放器 - 构建脚本
REM ========================================

echo ======================================
echo 开始构建音乐播放器...
echo ======================================
echo.

REM 1. 构建前端应用
echo [1/4] 构建 Vue 前端应用...
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
echo [2/4] 复制前端文件到 WPF 项目...
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
echo [3/4] 还原 .NET 依赖...
cd windesktop
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 还原依赖失败！
    pause
    exit /b 1
)
echo ✓ 依赖还原完成
echo.

REM 4. 构建 .NET 项目
echo [4/4] 构建 .NET 桌面应用...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo 错误: .NET 项目构建失败！
    pause
    exit /b 1
)
echo ✓ .NET 项目构建完成
echo.

echo ======================================
echo 构建完成！
echo.
echo 可执行文件位置：
echo windesktop\bin\Release\net8.0-windows\MusicPlayer.exe
echo ======================================
pause
