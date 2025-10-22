using System.Windows;
using System.Threading;

namespace MusicPlayer;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : System.Windows.Application
{
    private static Mutex? _mutex = null;
    private const string MutexName = "MusicPlayer_SingleInstance_Mutex_FistBaoZi";

    protected override void OnStartup(StartupEventArgs e)
    {
        // 检查是否已有实例在运行
        _mutex = new Mutex(true, MutexName, out bool createdNew);

        if (!createdNew)
        {
            // 已有实例在运行，显示提示并退出
            System.Windows.MessageBox.Show(
                "在线音乐播放器已经在运行中！\n\n请检查系统托盘或任务栏。",
                "提示",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
            
            // 释放互斥体并退出
            _mutex?.Dispose();
            _mutex = null;
            Current.Shutdown();
            return;
        }

        base.OnStartup(e);

        // 全局异常处理
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // 释放互斥体
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        _mutex = null;
        
        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        System.Windows.MessageBox.Show(
            $"发生未处理的异常：\n\n{exception?.Message}\n\n{exception?.StackTrace}",
            "错误",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error
        );
    }

    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show(
            $"发生UI线程异常：\n\n{e.Exception.Message}",
            "错误",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error
        );
        e.Handled = true;
    }
}
