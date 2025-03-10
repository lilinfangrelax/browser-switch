using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static browser_switch.NativeMethods;

namespace browser_switch
{
    static class Program
    {
        private const string AppGuid = "BrowserSwitch-1-22-333";
        public const int WM_SHOW_AND_PROCESS = 0x0418;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (Mutex mutex = new Mutex(true, AppGuid, out bool isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    SendArgsToRunningInstance(args);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow(args));
            }
        }
        private static void SendArgsToRunningInstance(string[] args)
        {
            IntPtr hWnd = NativeMethods.FindWindow(null, "browser-switch");
            string combinedArgs = string.Join("|", args);
            if (hWnd != IntPtr.Zero)
            {
                CopyDataStruct cds = new CopyDataStruct
                {
                    dwData = IntPtr.Zero,
                    cbData = (combinedArgs.Length + 1) * 2,
                    lpData = Marshal.StringToHGlobalUni(combinedArgs)
                };
                SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
                Marshal.FreeHGlobal(cds.lpData);
            }
        }
    }

    // Windows API
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            IntPtr hWnd, int msg, IntPtr wParam, ref CopyDataStruct lParam
        );
        public const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
    }
}
