using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace WinQuickTools.Services
{
    public static class AutostartService
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "WinQuickTools";

        public static bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
            var v = key?.GetValue(ValueName) as string;
            return !string.IsNullOrWhiteSpace(v);
        }

        public static void SetEnabled(bool enabled)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true)
                          ?? Registry.CurrentUser.CreateSubKey(RunKey, true);

            if (!enabled)
            {
                key.DeleteValue(ValueName, false);
                return;
            }

            // single-file/installed 모두 안전: 현재 실행 파일 경로
            var exePath = Process.GetCurrentProcess().MainModule!.FileName!;
            key.SetValue(ValueName, $"\"{exePath}\"", RegistryValueKind.String);
        }
    }
}