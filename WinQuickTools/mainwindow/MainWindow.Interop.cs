using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int SendMessageTimeout(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            string lParam,
            SendMessageTimeoutFlags flags,
            int timeout,
            out IntPtr lpdwResult);

        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_ABORTIFHUNG = 0x2
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            uint fsModifiers,
            uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id);
    }
}