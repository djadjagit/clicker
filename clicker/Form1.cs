using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;



namespace clicker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [DllImport("user32.dll")]
        public static extern void keybd_event(Keys bVk, byte bScan, UInt32 dwFlags, IntPtr dwExtraInfo);
        public const UInt32 KEYEVENTF_EXTENDEDKEY = 0x01;
        public const UInt32 KEYEVENTF_KEYUP = 0x02;
        int flag = 0;
        Int32 i = 0;

        public Form1()
        {
            InitializeComponent();
            KeyboardHook.Start();
            KeyboardHook.KeyboardAction += new KeyEventHandler(EventCtrl);
        }

        private void EventCtrl(object sender, KeyEventArgs e)
        {
            //            textBox1.Text = e.KeyValue.ToString()+"; Alt:"+e.Alt.ToString() + "; Ctrl:" + e.Control.ToString() + "; Shift:" + e.Shift.ToString();
            try
            {
                if (e.KeyValue == 163) //правый Ctrl включает кликалку
                {
                    flag = 1;
                    i = 0;
                    timer1.Enabled = true;
                    notifyIcon1.Icon = Resource1.NormalIcon;
                    toolStripTextBox1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString();
                    notifyIcon1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
                }
                if (e.KeyValue == 162) //левый Ctrl выключает кликалку
                {
                    flag = 0;
                    timer1.Enabled = false;
                    notifyIcon1.Icon = Resource1.DeletedIcon;
                    toolStripTextBox1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString();
                    notifyIcon1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
                }
                if (e.KeyValue == 38) //Up увеличивает интервал клика
                {
                    timer1.Interval = timer1.Interval + 100;
                    toolStripTextBox1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString();
                    notifyIcon1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
                }
                if (e.KeyValue == 40) //Down уменьшает интервал клика
                {
                    timer1.Interval = timer1.Interval - 100;
                    toolStripTextBox1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString();
                    notifyIcon1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                        "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
                }

            }
            catch (Exception err)
            {
                File.AppendAllText("log_clicker.txt", "EventCtrl;" + DateTime.Now.ToLongTimeString() + ";" + err.Message + "\r\n");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            i++;
            try
            {
                File.AppendAllText("log_clicker.txt", DateTime.Now.ToLongTimeString() + ";" + i.ToString() + "\r\n");
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                this.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                    "; Y:" + Cursor.Position.Y.ToString();
                notifyIcon1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                    "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
            }
            catch (Exception err)
            {
                File.AppendAllText("log_clicker.txt", "timer1_Tick;" + DateTime.Now.ToLongTimeString()+";"+err.Message + "\r\n");
            }
            if (flag == 1) timer1.Enabled = true;
        }

        public static class KeyboardHook
        {
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private static LowLevelKeyboardProc _proc = HookCallback;
            private static IntPtr _hookID = IntPtr.Zero;

            public static event KeyEventHandler KeyboardAction = delegate { };

            public static void Start()
            {
                _hookID = SetHook(_proc);
            }
            public static void stop()
            {
                UnhookWindowsHookEx(_hookID);
            }

            private static IntPtr SetHook(LowLevelKeyboardProc proc)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
                {
                    int vkCode = Marshal.ReadInt32(lParam);
//                    if ((vkCode > 36) && (vkCode < 41))
                    if ((vkCode == 38) || (vkCode == 40) || (vkCode == 162) || (vkCode == 163))
                    {
                        KeyEventArgs key = new KeyEventArgs((Keys)vkCode);
                        KeyboardAction(null, key);
                        return (IntPtr)1;
                    }
                }
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Resource1.DeletedIcon;
            toolStripTextBox1.Text = i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                "; Y:" + Cursor.Position.Y.ToString();
            notifyIcon1.Text= i.ToString() + ";" + timer1.Interval.ToString() + ";" + flag.ToString() + "; X:" + Cursor.Position.X.ToString() +
                "; Y:" + Cursor.Position.Y.ToString() + "\r\nВкл - правый Ctrl; Выкл - левый Ctrl";
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
        }
    }
}
