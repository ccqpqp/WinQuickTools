using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows;
using WinQuickTools.Services;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        // ---------- (1) 확장자 표시 토글 ----------
        private void ShowExtension_Click(object sender, RoutedEventArgs e)
        {
            const string keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

            int hide = (Registry.GetValue(keyPath, "HideFileExt", 1) as int?) ?? 1;
            int next = (hide == 0) ? 1 : 0;

            Registry.SetValue(keyPath, "HideFileExt", next, RegistryValueKind.DWord);

            RefreshExplorerShellState();
            UpdateStatusFromSystem();

            bool isOn = next == 0;

            ToastService.Show(
                "확장자 표시",
                isOn ? "켜짐" : "꺼짐",
                isOn);
        }

        private static bool IsFileExtensionShown()
        {
            const string keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            int hide = (Registry.GetValue(keyPath, "HideFileExt", 1) as int?) ?? 1;
            return hide == 0;
        }

        // ---------- (2) 숨김 파일 표시 토글 ----------
        private void ToggleHidden_Click(object sender, RoutedEventArgs e)
        {
            const string keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

            int current = (Registry.GetValue(keyPath, "Hidden", 2) as int?) ?? 2;
            int next = (current == 1) ? 2 : 1;

            Registry.SetValue(keyPath, "Hidden", next, RegistryValueKind.DWord);

            RefreshExplorerShellState();
            UpdateStatusFromSystem();

            bool isOn = next == 1;

            ToastService.Show(
                "숨김 파일 표시",
                isOn ? "켜짐" : "꺼짐",
                isOn);
        }

        private static bool IsHiddenFilesShown()
        {
            const string keyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            int hidden = (Registry.GetValue(keyPath, "Hidden", 2) as int?) ?? 2;
            return hidden == 1;
        }

        // ---------- (3) 탐색기 재시작 ----------
        private void RestartExplorer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var p in Process.GetProcessesByName("explorer"))
                {
                    try { p.Kill(); } catch { }
                }

                Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
                SetStatus("탐색기 재시작 완료");
            }
            catch
            {
                SetStatus("탐색기 재시작 실패");
            }
        }

        // ---------- (4) 경로 복사 ----------
        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            string? path = GetExplorerSelectionOrFolderPath();

            if (string.IsNullOrWhiteSpace(path))
            {
                SetStatus("선택된 항목/탐색기 창 없음");
                return;
            }

            Clipboard.SetText(path);
            SetStatus($"경로 복사됨: {path}");
        }

        private static string? GetExplorerSelectionOrFolderPath()
        {
            try
            {
                Type? t = Type.GetTypeFromProgID("Shell.Application");
                if (t == null) return null;

                dynamic shell = Activator.CreateInstance(t)!;
                dynamic windows = shell.Windows();

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    dynamic w = windows.Item(i);
                    string fullName = (string)w.FullName;

                    if (!fullName.EndsWith("explorer.exe", StringComparison.OrdinalIgnoreCase))
                        continue;

                    dynamic doc = w.Document;

                    dynamic selected = doc.SelectedItems();
                    if (selected != null && selected.Count > 0)
                    {
                        dynamic item0 = selected.Item(0);
                        return (string)item0.Path;
                    }

                    dynamic folder = doc.Folder;
                    return (string)folder.Self.Path;
                }
            }
            catch { }

            return null;
        }

        // ---------- 파일 목록 ----------
        private void FileList_Click(object sender, RoutedEventArgs e) => SetStatus("파일 목록 기능 준비중");

        // ---------- Explorer 반영 ----------
        private void RefreshExplorerShellState()
        {
            const int HWND_BROADCAST = 0xffff;
            const int WM_SETTINGCHANGE = 0x001A;

            SendMessageTimeout(
                (IntPtr)HWND_BROADCAST,
                WM_SETTINGCHANGE,
                IntPtr.Zero,
                "ShellState",
                SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
                200,
                out _);
        }
    }
}