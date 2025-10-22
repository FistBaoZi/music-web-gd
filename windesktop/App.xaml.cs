using System.Windows;

namespace MusicPlayer;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 全局异常处理
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        MessageBox.Show(
            $"发生未处理的异常：\n\n{exception?.Message}\n\n{exception?.StackTrace}",
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"发生UI线程异常：\n\n{e.Exception.Message}",
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
        e.Handled = true;
    }
}
