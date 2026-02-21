using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;

namespace WinQuickTools.Features
{
    public static class SystemRestore
    {
        public static void RestoreAll()
        {
            // =========================
            // 확인 팝업 (필수)
            // =========================
            var result = MessageBox.Show(
                "모든 설정을 초기 상태로 되돌립니다.\n계속하시겠습니까?",
                "설정 복구",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // -------------------------
                // 확장자 숨김 (기본값)
                // -------------------------
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "HideFileExt",
                    1,
                    RegistryValueKind.DWord);

                // -------------------------
                // 숨김파일 숨김 (기본값)
                // -------------------------
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                    "Hidden",
                    2,
                    RegistryValueKind.DWord);

                // -------------------------
                // Clipboard History OFF
                // -------------------------
                Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Clipboard",
                    "EnableClipboardHistory",
                    0,
                    RegistryValueKind.DWord);

                // -------------------------
                // WinQuickTools 우클릭 제거
                // -------------------------
                Registry.CurrentUser.DeleteSubKeyTree(
                    @"Software\Classes\*\shell\WinQuickTools",
                    false);

                // -------------------------
                // 탐색기 재시작 (적용)
                // -------------------------
                RestartExplorer();

                MessageBox.Show(
                    "모든 설정이 초기 상태로 복구되었습니다.",
                    "완료",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "복구 실패",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static void RestartExplorer()
        {
            foreach (var p in Process.GetProcessesByName("explorer"))
                p.Kill();

            Process.Start("explorer.exe");
        }
    }
}