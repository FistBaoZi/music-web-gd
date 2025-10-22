@echo off
chcp 65001 >nul
REM ========================================
REM 音乐播放器 - 运行脚本（开发模式）
REM ========================================

echo ======================================
echo 运行音乐播放器（开发模式）...
echo ======================================
echo.

REM 检查是否已构建
if not exist "bin\Debug\net9.0-windows\MusicPlayer.exe" (
    echo 检测到未构建，开始构建...
    call build.bat
    if %ERRORLEVEL% NEQ 0 (
        echo 构建失败，无法运行
        pause
        exit /b 1
    )
)

REM 运行应用
echo 启动应用...
cd bin\Debug\net9.0-windows
start MusicPlayer.exe
cd ..\..\..

echo ✓ 应用已启动
echo.
pause
