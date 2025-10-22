# 图标文件说明

请将应用图标文件命名为 `icon.ico` 并放在此目录。

## 图标要求

- 文件名：`icon.ico`
- 格式：ICO
- 推荐尺寸：256x256, 128x128, 64x64, 48x48, 32x32, 16x16（多尺寸图标）

## 在线工具

可以使用以下在线工具将 PNG 转换为 ICO：
- https://convertio.co/zh/png-ico/
- https://www.icoconverter.com/
- https://cloudconvert.com/png-to-ico

## 临时方案

如果暂时没有图标，可以：
1. 从 `../public/` 文件夹复制现有图标
2. 或者注释掉项目文件中的图标引用：
   - `MainWindow.xaml` 中的 `Icon="icon.ico"`
   - `MusicPlayer.csproj` 中的 `<ApplicationIcon>icon.ico</ApplicationIcon>`
