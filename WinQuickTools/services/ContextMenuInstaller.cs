using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace WinQuickTools.Services
{
    internal static class ContextMenuInstaller
    {
        private const string MENU_NAME = "WinQuickTools";

        public static void Install()
        {
            string exe = Process.GetCurrentProcess().MainModule!.FileName!;

            // 폴더 우클릭
            using (var key = Registry.CurrentUser.CreateSubKey(
                $@"Software\Classes\Directory\shell\{MENU_NAME}"))
            {
                key!.SetValue("", "WinQuickTools 실행");
                key.SetValue("Icon", exe);

                using var cmd = key.CreateSubKey("command");
                cmd!.SetValue("", $"\"{exe}\" \"%1\"");
            }

            // 배경 우클릭 (폴더 안 빈공간)
            using (var key = Registry.CurrentUser.CreateSubKey(
                $@"Software\Classes\Directory\Background\shell\{MENU_NAME}"))
            {
                key!.SetValue("", "WinQuickTools 실행");
                key.SetValue("Icon", exe);

                using var cmd = key.CreateSubKey("command");
                cmd!.SetValue("", $"\"{exe}\" \"%V\"");
            }
        }

        public static void Uninstall()
        {
            Registry.CurrentUser.DeleteSubKeyTree(
                $@"Software\Classes\Directory\shell\{MENU_NAME}", false);

            Registry.CurrentUser.DeleteSubKeyTree(
                $@"Software\Classes\Directory\Background\shell\{MENU_NAME}", false);
        }
    }
}