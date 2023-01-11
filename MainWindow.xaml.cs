using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace L4D2FontLoader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fontINIPath = ".\\MacType\\L4D2.ini";
        private const string macLoaderPath = ".\\MacType\\MacLoader.exe";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Main_Loaded(object sender, RoutedEventArgs e)
        {
            // 读取对应配置文件
            TextBox_L4D2ExecPath.Text = IniHelper.ReadValue("Main", "L4D2ExecPath");
            TextBox_L4D2RunArgs.Text = IniHelper.ReadValue("Main", "L4D2RunArgs");
            TextBox_CustomFontName.Text = IniHelper.ReadValue("Main", "CustomFontName");

            if (string.IsNullOrWhiteSpace(TextBox_L4D2RunArgs.Text))
                TextBox_L4D2RunArgs.Text = "-steam -novid -language schinese";
            if (string.IsNullOrWhiteSpace(TextBox_CustomFontName.Text))
                TextBox_CustomFontName.Text = "楷体";
        }

        private void Window_Main_Closing(object sender, CancelEventArgs e)
        {
            IniHelper.WriteValue("Main", "L4D2ExecPath", TextBox_L4D2ExecPath.Text.Trim());
            IniHelper.WriteValue("Main", "L4D2RunArgs", TextBox_L4D2RunArgs.Text.Trim());
            IniHelper.WriteValue("Main", "CustomFontName", TextBox_CustomFontName.Text.Trim());
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.OriginalString);
            e.Handled = true;
        }

        private void Button_CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WriteCustonFontName(string fontName)
        {
            IniHelper.WriteValue("FontSubstitutes@L4D2", "SimSun", fontName, fontINIPath);
            IniHelper.WriteValue("FontSubstitutes@L4D2", "NSimSun", fontName, fontINIPath);
            IniHelper.WriteValue("FontSubstitutes@L4D2", "Tahoma", fontName, fontINIPath);
        }

        private void Button_RunL4D2WithArgs_Click(object sender, RoutedEventArgs e)
        {
            var execPath = TextBox_L4D2ExecPath.Text.Trim();
            var runArgs = TextBox_L4D2RunArgs.Text.Trim();
            var fontName = TextBox_CustomFontName.Text.Trim();

            if (string.IsNullOrWhiteSpace(execPath))
            {
                TextBlock_Logger.Text = "警告：求生之路2主程序路径不能为空";
                TextBlock_Logger.Foreground = Brushes.Orange;
                return;
            }

            if (string.IsNullOrWhiteSpace(runArgs))
            {
                TextBlock_Logger.Text = "警告：求生之路2运行参数不能为空";
                TextBlock_Logger.Foreground = Brushes.Orange;
                return;
            }

            if (string.IsNullOrWhiteSpace(fontName))
            {
                TextBlock_Logger.Text = "警告：求生之路2字体名称不能为空";
                TextBlock_Logger.Foreground = Brushes.Orange;
                return;
            }

            if (!File.Exists(execPath))
            {
                TextBlock_Logger.Text = "警告：求生之路2主程序文件不存在或路径输入错误";
                TextBlock_Logger.Foreground = Brushes.Orange;
                return;
            }

            try
            {
                WriteCustonFontName(fontName);

                var process = new Process();
                process.StartInfo.FileName = macLoaderPath;
                process.StartInfo.Arguments = $"\"{execPath}\" {runArgs}";
                process.StartInfo.UseShellExecute = false;
                process.Start();
            }
            catch (Exception ex)
            {
                TextBlock_Logger.Text = $"异常：{ex.Message}";
                TextBlock_Logger.Foreground = Brushes.Red;
                return;
            }

            this.Close();
        }
    }
}
