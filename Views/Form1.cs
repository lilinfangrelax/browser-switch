using browser_switch.Services;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static browser_switch.NativeMethods;

namespace browser_switch
{
    public partial class MainWindow : Form
    {
        private String[] _args;
        private RouterService _service;
        public MainWindow(String[] args)
        {
            InitializeComponent();
            InitializeTray();
            this.Text = Config.exeName;
            _args = args;
            _service = new RouterService();

            CheckRegistryAndDefaultBrowser();

            Logger.AppendText($"配置文件路径: {Config.exeDir}\\router.txt");
            Logger.AppendText(Environment.NewLine);
            Logger.AppendText(Environment.NewLine);

            CheckChromeFirefoxEdgeBrowser();
        }

        public void CheckRegistryAndDefaultBrowser()
        {
            RegistryService _registry_service = new RegistryService();
            bool ret = _registry_service.CheckIfExistRegistryKey();
            Logger.AppendText($"注册表检查: {ret}");
            Logger.AppendText(Environment.NewLine);
            if (!ret)
            {
                Logger.AppendText("目录将会打开. 请鼠标双击文件进行注册: Browser-Switch-Register.reg");
                Logger.AppendText(Environment.NewLine);
                Logger.AppendText(Environment.NewLine);
                _registry_service.CreateRegistry();
            }
            bool is_default_browser = _registry_service.CheckWindowsDefaultBrowser();
            Logger.AppendText($"默认浏览器检查: {is_default_browser}");
            Logger.AppendText(Environment.NewLine);
            if (!is_default_browser)
            {
                Logger.AppendText("请将本程序设置为http/https的默认打开应用（注册表先注册）");
                Logger.AppendText(Environment.NewLine);
            }

            Logger.AppendText(Environment.NewLine);
        }

        public void CheckChromeFirefoxEdgeBrowser()
        {
            var detector = new BrowserDetectorService();
            foreach (var browser in detector.DetectBrowsers())
            {

                if (browser.IsInstalled)
                {
                    Logger.AppendText($"浏览器: {browser.Name}");
                    Logger.AppendText(Environment.NewLine);
                    //Logger.AppendText($"安装: {browser.IsInstalled}");
                    //Logger.AppendText(Environment.NewLine);
                    Logger.AppendText($"路径: {browser.InstallPath}");
                    Logger.AppendText(Environment.NewLine);
                    Logger.AppendText("用户配置: ");
                    foreach (var profile in browser.Profiles)
                    {
                        if (browser.Name == "Mozilla Firefox")
                            Logger.AppendText($"{profile.Split('/')[profile.Split('/').Length - 1]} | ");
                        else
                            Logger.AppendText($"{profile.Split('\\')[profile.Split('\\').Length - 1]} | ");

                    }
                    Logger.AppendText(Environment.NewLine);
                    Logger.AppendText(Environment.NewLine);
                }

            }

        }

        private void InitializeTray()
        {
            notifyIcon1.Text = "后台服务正在运行";

            var menu = new ContextMenuStrip();
            menu.Items.Add("打开窗口", null, (s, e) => RestoreWindow());
            menu.Items.Add("配置文件目录", null, (s, e) => ShowConfigDir());
            menu.Items.Add("退出", null, (s, e) => Application.Exit());

            notifyIcon1.ContextMenuStrip = menu;
            notifyIcon1.MouseDoubleClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) RestoreWindow();
            };
        }

        private void ShowConfigDir()
        {
            System.Diagnostics.Process.Start("explorer.exe", Config.exeDir);
        }
        private bool _windowState = true;
        private void RestoreWindow()
        {
            // toggle window state
            // add feature:  hide in tray when close the window
            // https://github.com/lilinfangrelax/browser-switch/issues/3
            if (_windowState)
            {
                this.Hide();
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
                _windowState = false;
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                _windowState = true;
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                _windowState = false;
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (_args.Length > 0)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string newMsg = $"[{timestamp}] URL: {string.Join(", ", _args)}";
                Logger.AppendText(newMsg);
                // logger add new line
                Logger.AppendText(Environment.NewLine);
                _service.Route(_args[0]);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                CopyDataStruct cds = (CopyDataStruct)Marshal.PtrToStructure(
                    m.LParam,
                    typeof(CopyDataStruct)
                );
                string data = Marshal.PtrToStringUni(cds.lpData);
                string[] newParams = data.Split('|');
                ProcessNewParams(newParams);
                this.WindowState = FormWindowState.Normal;
                // fix issue: When you click the URL, the minimized window is always popup. 
                // https://github.com/lilinfangrelax/browser-switch/issues/4
                //this.BringToFront();
            }
            base.WndProc(ref m);
        }


        private void ProcessNewParams(string[] args)
        {
            if (args.Length == 1 && "".Equals(string.Join(", ", args)))
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                _windowState = true;
                this.BringToFront();
                return;
            }

            if (args.Length > 0)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string newMsg = $"[{timestamp}] URL: {string.Join(", ", args)}";
                Logger.AppendText(newMsg);
                Logger.AppendText(Environment.NewLine);
                _service.Route(args[0]);
            }
        }
    }
}
