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
            this.Text = "browser-switch";
            _args = args;
            _service = new RouterService();
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
                this.BringToFront();
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
