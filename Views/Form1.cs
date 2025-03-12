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
            this.Text = "browser-switch";
            _args = args;
            _service = new RouterService();
            CheckRegistry();
        }

        public void CheckRegistry()
        {
            RegistryService _registry_service = new RegistryService();
            bool ret = _registry_service.CheckIfExistRegistryKey();
            Logger.AppendText($"Check registry: {ret}");
            Logger.AppendText(Environment.NewLine);
            if (!ret)
            {
                Logger.AppendText($"The directory will be opened. Please DOUBLE Click file: Browser-Switch-Register.reg");
                Logger.AppendText(Environment.NewLine);
                _registry_service.CreateRegistry();
            }
        }

        private void InitializeTray()
        {
            notifyIcon1.Text = "Background Service Running";

            var menu = new ContextMenuStrip();
            menu.Items.Add("Show Window", null, (s, e) => RestoreWindow());
            menu.Items.Add("Exit", null, (s, e) => Application.Exit());

            notifyIcon1.ContextMenuStrip = menu;
            notifyIcon1.MouseDoubleClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) RestoreWindow();
            };
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
                string newMsg = $"[{timestamp}] Received URL: {string.Join(", ", _args)}";
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
            if (args.Length > 0)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string newMsg = $"[{timestamp}] Received URL: {string.Join(", ", args)}";
                Logger.AppendText(newMsg);
                Logger.AppendText(Environment.NewLine);
                _service.Route(args[0]);
            }
        }
    }
}
