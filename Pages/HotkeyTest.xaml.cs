using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;
using Windows.System;

static class WinApi
{

    public const int WM_HOTKEY = 0x312;

    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASS
    {
        public uint style;

        public WindowProcedureHandler lpfnWndProc;

        public int cbClsExtra;

        public int cbWndExtra;

        public IntPtr hInstance;

        public IntPtr hIcon;

        public IntPtr hCursor;

        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    internal static class Kernel32
    {
        public const string DllName = "kernel32.dll";

        [DllImport(DllName)]
        public static extern ushort GlobalAddAtomA([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(DllName)]
        public static extern ushort GlobalFindAtomA([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(DllName)]
        public static extern ushort GlobalDeleteAtom(ushort atom);
    }

    public delegate nint WindowProcedureHandler(nint hWnd, int msg, nint wParam, nint lParam);

    internal static class User32
    {
        public const string DllName = "user32.dll";

        [DllImport(User32.DllName)]
        public static extern nint CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
    int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam);


        [DllImport(User32.DllName, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass([In] ref WNDCLASS lpWndClass);

        [DllImport(User32.DllName)]
        public static extern nint DefWindowProc(nint hWnd, int uMsg, nint wParam, nint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(nint hWnd, int id);

        [DllImport(User32.DllName, SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);
    }

}

namespace Fantastical.App.Pages
{

    internal sealed partial class HotkeyTest : Page
    {
        private nint _hwnd;

        public HotkeyTest()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            WinApi.WNDCLASS wc;

            wc.style = 0;
            wc.lpfnWndProc = this.WindowProc;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = "Fantastical.Messaging";

            // Register the window class
            WinApi.User32.RegisterClass(ref wc);

            // Create the message window
            this._hwnd = WinApi.User32.CreateWindowEx(
                0,
                wc.lpszClassName,
                string.Empty,
                0,
                0,
                0,
                1,
                1,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (this._hwnd == IntPtr.Zero)
            {
                throw new Win32Exception("Message window handle was not a valid pointer");
            }

            bool ok = WinApi.User32.RegisterHotKey(this._hwnd, 1, (uint)(VirtualKeyModifiers.Control) 
                //| 0x4000
                , (uint)VirtualKey.B);

            if (!ok)
            {
                throw new Win32Exception("bogus");
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WinApi.WM_HOTKEY)
            {
            }

            return WinApi.User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            WinApi.User32.DestroyWindow(this._hwnd);
            this._hwnd = 0;
        }
    }

}