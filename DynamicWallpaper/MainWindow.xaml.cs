using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace DynamicWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        private extern static long GetLastError();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr IParam);
        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        private extern static IntPtr FindWindowA(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowExA")]
        private extern static IntPtr FindWindowExA(IntPtr hWndParent,  IntPtr hWndChildAfter,  string lpszClass,  string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        private extern static IntPtr GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "GetParent")]
        private extern static IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private extern static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        private IntPtr BackGround = IntPtr.Zero;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            IntPtr hwnd = FindWindowA("progman", "Program Manager");
            Debug.WriteLine($"hwnd:{hwnd:X}");
            IntPtr worker = IntPtr.Zero;
            SendMessage(hwnd, 0x052C, IntPtr.Zero, IntPtr.Zero);
            do
            {
                worker = FindWindowExA(IntPtr.Zero, worker, "WorkerW", null);
                Debug.WriteLine($"workers:{worker:X}");
                if (worker != IntPtr.Zero)
                {
                    StringBuilder buff = new StringBuilder(200);
                    IntPtr ret = GetClassName(worker, buff, buff.Capacity);
                    Debug.WriteLine($"return:{ret:X} ClassName:{buff}");
                    if (ret == IntPtr.Zero)
                    {
                        long err = GetLastError();
                        Debug.WriteLine($"Error: {err}");
                    }
                }
                if (GetParent(worker) == hwnd)
                {
                    BackGround = worker;
                }
            }
            while (worker != IntPtr.Zero);
            Debug.WriteLine($"background:{BackGround:X}");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wihelper = new WindowInteropHelper(this);
            IntPtr current = wihelper.Handle;
            Debug.WriteLine($"current:{current:X} background:{BackGround:X}");
            SetParent(current, BackGround);
        }
    }
}
