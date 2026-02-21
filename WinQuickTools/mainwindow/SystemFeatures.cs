using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using WinQuickTools.Windows;
using System.Windows.Forms;

namespace WinQuickTools.Features
{
    internal static class SystemFeatures
    {
        // ===============================
        // 공통 실행 헬퍼
        // ===============================
        private static void RunCmd(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + args,
                Verb = "runas",
                CreateNoWindow = true,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        // =====================================================
        // dnsflush
        // =====================================================
        public static void DnsFlush(string? _ = null)
        {
            if (!EtDialog.Confirm(
                "인터넷 문제 해결",
                "DNS 캐시를 초기화합니다.\n\n계속하시겠습니까?"))
                return;

            RunCmd("ipconfig /flushdns");

            EtDialog.Alert("완료", "네트워크 캐시 초기화 완료");
        }

        // =====================================================
        // clipboardhistory (Win+V 활성화)
        // =====================================================
        public static void ToggleClipboardHistory(string? _ = null)
        {
            const string key =
                @"HKEY_CURRENT_USER\Software\Microsoft\Clipboard";

            object? val = Registry.GetValue(
                key,
                "EnableClipboardHistory",
                0);

            bool enabled = (val is int i && i == 1);

            string actionText = enabled ? "비활성화" : "활성화";

            if (!EtDialog.Confirm(
                "복사 여러개 사용",
                $"클립보드 기록을 {actionText} 합니다.\n\n계속하시겠습니까?"))
                return;

            Registry.SetValue(
                key,
                "EnableClipboardHistory",
                enabled ? 0 : 1,
                RegistryValueKind.DWord);

            // 설정 화면 열어서 체감 주기
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "ms-settings:clipboard",
                    UseShellExecute = true
                });
            }
            catch { }

            EtDialog.Alert(
                "완료",
                enabled
                    ? "클립보드 기록이 꺼졌습니다."
                    : "클립보드 기록이 켜졌습니다.\nWin+V로 확인하세요.");
        }

        // =====================================================
        // contextmenu (Shift 우클릭 확장 메뉴 항상 표시)
        // =====================================================
        public static void EnableFullContextMenu(string? _ = null)
        {
            if (!EtDialog.Confirm(
                "우클릭 메뉴 사용",
                "Windows 11 전체 우클릭 메뉴를 항상 표시합니다.\n\n계속하시겠습니까?"))
                return;

            string key =
                @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";

            using var k = Registry.CurrentUser.CreateSubKey(key);
            k?.SetValue("", "");

            EtDialog.Alert(
                "완료",
                "적용되었습니다.\n탐색기 재시작 후 적용됩니다.");
        }

        // =====================================================
        // downloadsorganize
        // =====================================================
        public static void OrganizeDownloads(string? _ = null)
        {
            if (!EtDialog.Confirm(
                "다운로드 폴더 정리",
                "파일 유형별 폴더로 자동 분류합니다.\n\n계속하시겠습니까?"))
                return;

            string downloads =
                Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.UserProfile),
                    "Downloads");

            var files = Directory.GetFiles(downloads);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToLower();

                string folder = ext switch
                {
                    ".jpg" or ".png" or ".jpeg" => "Images",
                    ".zip" or ".rar" => "Archives",
                    ".pdf" => "PDF",
                    ".mp4" or ".mkv" => "Videos",
                    ".exe" => "Programs",
                    _ => "Others"
                };

                string targetDir = Path.Combine(downloads, folder);
                Directory.CreateDirectory(targetDir);

                string dest = Path.Combine(targetDir,
                    Path.GetFileName(file));

                if (!File.Exists(dest))
                    File.Move(file, dest);
            }

            EtDialog.Alert("완료", "다운로드 폴더 정리 완료");
        }

        // =====================================================
        // screenshotpath
        // =====================================================
        public static void ChangeScreenshotPath(string? _ = null)
        {
            if (!EtDialog.Confirm(
                "스크린샷 저장 위치 변경",
                "스크린샷 저장 폴더를 직접 선택합니다.\n\n계속하시겠습니까?"))
                return;

            using var dialog = new FolderBrowserDialog
            {
                Description = "스크린샷 저장 폴더 선택",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string selectedPath = dialog.SelectedPath;

            try
            {
                const string key =
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders";

                // Windows Screenshot GUID
                const string screenshotGuid =
                    "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}";

                Registry.SetValue(
                    key,
                    screenshotGuid,
                    selectedPath,
                    RegistryValueKind.ExpandString);

                EtDialog.Alert(
                    "완료",
                    $"스크린샷 저장 위치 변경됨\n\n{selectedPath}\n\n※ 재로그인 또는 탐색기 재시작 후 완전 적용됩니다.");
            }
            catch (Exception ex)
            {
                EtDialog.Alert("오류", ex.Message);
            }
        }
    }
}