using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace MusicPlayer;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // 启用硬件加速，减少闪烁
        System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.HighQuality);
        
        // 禁用窗口大小调整时的重绘优化
        this.UseLayoutRounding = true;
        
        InitializeWebView();
    }

    private async void InitializeWebView()
    {
        try
        {
            // 设置 WebView2 用户数据文件夹路径
            var userDataFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MusicPlayer",
                "WebView2"
            );

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await webView.EnsureCoreWebView2Async(env);

            // 配置 WebView2 设置
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = true; // 开发时启用，发布时可改为 false
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            
            // 禁用滚动条，减少闪烁
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            // 监听导航完成事件
            webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;

            // 监听控制台消息（用于调试）
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // 加载前端应用
            LoadFrontendApp();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"初始化 WebView2 失败：{ex.Message}\n\n请确保已安装 Microsoft Edge WebView2 Runtime。",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            Application.Current.Shutdown();
        }
    }

    private void LoadFrontendApp()
    {
        try
        {
            // 获取前端应用路径
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var distPath = System.IO.Path.Combine(baseDir, "wwwroot");

            if (!System.IO.Directory.Exists(distPath))
            {
                MessageBox.Show(
                    $"未找到前端应用文件！\n\n路径：{distPath}\n\n请先运行构建脚本生成前端文件。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Application.Current.Shutdown();
                return;
            }

            // 使用虚拟主机名映射本地文件夹
            // 这样可以避免 file:// 协议的限制
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.local",
                distPath,
                CoreWebView2HostResourceAccessKind.Allow
            );

            // 导航到前端应用
            webView.CoreWebView2.Navigate("https://app.local/index.html");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"加载前端应用失败：{ex.Message}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        // 导航完成后隐藏加载提示，显示 WebView
        LoadingPanel.Visibility = Visibility.Collapsed;
        webView.Visibility = Visibility.Visible;

        if (!e.IsSuccess)
        {
            MessageBox.Show(
                $"页面加载失败！\n错误码：{e.WebErrorStatus}",
                "导航错误",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        // 接收来自前端的消息
        var message = e.TryGetWebMessageAsString();
        System.Diagnostics.Debug.WriteLine($"来自前端的消息: {message}");

        // 可以在这里处理前端发送的消息
        // 例如：下载功能、系统托盘等
    }

    /// <summary>
    /// 向前端发送消息的方法（供扩展使用）
    /// </summary>
    public void SendMessageToFrontend(string message)
    {
        if (webView.CoreWebView2 != null)
        {
            webView.CoreWebView2.PostWebMessageAsString(message);
        }
    }
}
