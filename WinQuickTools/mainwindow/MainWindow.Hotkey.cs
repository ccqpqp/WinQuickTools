using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using WinQuickTools.Services;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;

            HwndSource source = HwndSource.FromHwnd(_windowHandle);
            source.AddHook(WndProc);
        }

        private void EnableF1Capture()
        {
            if (_f1CaptureEnabled)
                return;

            // 🔥 핸들 보장
            if (_windowHandle == IntPtr.Zero)
                _windowHandle = new WindowInteropHelper(this).Handle;

            bool ok = RegisterHotKey(_windowHandle, HOTKEY_ID, 0, 0x70);

            _f1CaptureEnabled = true;

            var item = FindItem("quickcapture");
            if (item != null)
                item.StatusText = "현재: 켜짐";

            ToastService.Show("빠른 캡처", "켜짐 (F1)", true);
        }

        private void DisableF1Capture()
        {
            if (!_f1CaptureEnabled)
                return;

            if (_windowHandle != IntPtr.Zero)
                UnregisterHotKey(_windowHandle, HOTKEY_ID);

            _f1CaptureEnabled = false;

            var item = FindItem("quickcapture");
            if (item != null)
                item.StatusText = "현재: 꺼짐";

            ToastService.Show("빠른 캡처", "꺼짐", false);
        }

        private void ToggleF1Capture()
        {
            if (_f1CaptureEnabled)
                DisableF1Capture();
            else
                EnableF1Capture();
        }

        private IntPtr WndProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                // Windows 기본 캡처 실행
                Process.Start("explorer.exe", "ms-screenclip:");
                handled = true;
            }

            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_f1CaptureEnabled)
                UnregisterHotKey(_windowHandle, HOTKEY_ID);

            base.OnClosed(e);
        }
    }
}