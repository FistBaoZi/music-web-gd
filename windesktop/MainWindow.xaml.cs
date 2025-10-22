using System.Windows;
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace MusicPlayer;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private NotifyIcon? notifyIcon;
    private bool isClosing = false;
    private System.Windows.Threading.DispatcherTimer? resizeTimer;

    // Windows API 用于优化窗口渲染
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;
    private const int WS_MINIMIZEBOX = 0x20000;

    public MainWindow()
    {
        InitializeComponent();
        
        // 启用硬件加速，减少闪烁
        System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.HighQuality);
        
        // 禁用窗口大小调整时的重绘优化
        this.UseLayoutRounding = true;

        // 初始化系统托盘
        InitializeNotifyIcon();

        // 初始化调整大小防抖动定时器
        InitializeResizeTimer();
        
        InitializeWebView();

        // 窗口加载完成后优化渲染
        this.Loaded += MainWindow_Loaded;
        this.SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 禁用窗口动画，减少闪烁
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd != IntPtr.Zero)
        {
            // 优化窗口样式
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, currentStyle | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);
        }
    }

    private void InitializeResizeTimer()
    {
        // 创建防抖动定时器，减少调整大小时的重绘次数
        resizeTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        resizeTimer.Tick += (s, e) =>
        {
            resizeTimer?.Stop();
            // 调整大小完成后，强制刷新 WebView
            if (webView.CoreWebView2 != null)
            {
                // 通知 WebView 调整大小完成
                webView.UpdateLayout();
            }
        };
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // 使用防抖动，避免频繁重绘
        resizeTimer?.Stop();
        resizeTimer?.Start();
    }

    private void InitializeNotifyIcon()
    {
        try
        {
            // 创建系统托盘图标
            notifyIcon = new NotifyIcon
            {
                Visible = false,
                Text = "在线音乐播放器"
            };

            // 尝试加载图标
            var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                notifyIcon.Icon = new Icon(iconPath);
            }
            else
            {
                // 使用默认图标
                notifyIcon.Icon = SystemIcons.Application;
            }

            // 双击托盘图标恢复窗口
            notifyIcon.DoubleClick += (s, e) =>
            {
                ShowWindow();
            };

            // 创建右键菜单
            var contextMenu = new ContextMenuStrip();
            
            var showMenuItem = new ToolStripMenuItem("显示主窗口");
            showMenuItem.Click += (s, e) => ShowWindow();
            contextMenu.Items.Add(showMenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += (s, e) =>
            {
                isClosing = true;
                System.Windows.Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitMenuItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化系统托盘图标失败: {ex.Message}");
        }
    }

    private void ShowWindow()
    {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Activate();
        if (notifyIcon != null)
        {
            notifyIcon.Visible = false;
        }
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        // 最小化时隐藏到系统托盘
        if (this.WindowState == WindowState.Minimized)
        {
            this.Hide();
            if (notifyIcon != null)
            {
                notifyIcon.Visible = true;
                // 不显示气泡通知，静默最小化到托盘
            }
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // 点击关闭按钮时最小化到托盘而不是关闭
        if (!isClosing)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }
        else
        {
            // 真正关闭时清理资源
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }
            
            resizeTimer?.Stop();
            resizeTimer = null;
        }
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
            System.Windows.MessageBox.Show(
                $"初始化 WebView2 失败：{ex.Message}\n\n请确保已安装 Microsoft Edge WebView2 Runtime。",
                "错误",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error
            );
            System.Windows.Application.Current.Shutdown();
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
                System.Windows.MessageBox.Show(
                    $"未找到前端应用文件！\n\n路径：{distPath}\n\n请先运行构建脚本生成前端文件。",
                    "错误",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
                System.Windows.Application.Current.Shutdown();
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
            System.Windows.MessageBox.Show(
                $"加载前端应用失败：{ex.Message}",
                "错误",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error
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
            System.Windows.MessageBox.Show(
                $"页面加载失败！\n错误码：{e.WebErrorStatus}",
                "导航错误",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning
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
