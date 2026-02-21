using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinQuickTools.Features;
using Microsoft.Win32;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide(); // 종료 X
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // 트레이에서 "종료" 눌렀을 때만 진짜 종료
            if (!_allowExit)
            {
                e.Cancel = true;
                Hide();
            }
        }


        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                e.Source == sender)
            {
                DragMove();
            }
        }

        private void SetStatus(string msg)
        {
            if (StatusText != null) StatusText.Text = msg;
        }

        private void UpdateStatusFromSystem()
        {
            bool extShown = IsFileExtensionShown();
            bool hiddenShown = IsHiddenFilesShown();

            string extText = extShown ? "현재: 표시중" : "현재: 숨김중";
            string hiddenText = hiddenShown ? "현재: 표시중" : "현재: 숨김중";

            SetStatus($"{extText} · {hiddenText}");

            var extItem = FindItem("ext");
            if (extItem != null) extItem.StatusText = extText;

            var hiddenItem = FindItem("hidden");
            if (hiddenItem != null) hiddenItem.StatusText = hiddenText;

            // ⭐ 추가 부분
            var captureItem = FindItem("quickcapture");
            if (captureItem != null)
            {
                captureItem.StatusText = _f1CaptureEnabled
                    ? "현재: 켜짐"
                    : "현재: 꺼짐";
            }
            var clipItem = FindItem("clipboardhistory");

            if (clipItem != null)
            {
                const string key =
                    @"Software\Microsoft\Clipboard";


                bool enabled = true;

                using var k = Registry.CurrentUser.OpenSubKey(key);
                if (k != null)
                {
                    object? v = k.GetValue("EnableClipboardHistory");
                    if (v is int i)
                        enabled = i == 1;
                }

                clipItem.StatusText =
                    enabled ? "현재: 켜짐" : "현재: 꺼짐";
            }
        }

        private void Feature_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            if (b.Tag is not string id) return;

            switch (id)
            {
                case "ext": ShowExtension_Click(sender, e); break;
                case "hidden": ToggleHidden_Click(sender, e); break;
                case "restart": RestartExplorer_Click(sender, e); break;
                case "copypath": CopyPath_Click(sender, e); break;
                case "restoreall": RestoreAll_Click(sender, e); break;
                // ⭐⭐⭐ 이거 추가
                case "quickcapture": ToggleF1Capture(); break;
                case "contextmenu": ToggleContextMenu(); break;

                case "exportlist": ExportList_Click(sender, e); break;
                case "renamebatch": RenameBatch_Click(sender, e); break;
                case "renumber": Renumber_Click(sender, e); break;
                case "fullpathcopy": FullPathCopy_Click(sender, e); break;

                case "dnsflush":
                    SystemFeatures.DnsFlush();
                    break;

                case "clipboardhistory":
                    SystemFeatures.ToggleClipboardHistory();
                    break;

                case "downloadsorganize":
                    SystemFeatures.OrganizeDownloads();
                    break;

                case "screenshotpath":
                    SystemFeatures.ChangeScreenshotPath();
                    break;
            }

            // ⭐⭐⭐ 이거 추가 (핵심)
            UpdateStatusFromSystem();
        }
        private void RestoreAll_Click(object sender, RoutedEventArgs e)
        {
            WinQuickTools.Features.SystemRestore.RestoreAll();
            UpdateStatusFromSystem();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow
            {
                Owner = this,
                Topmost = true,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            // ⭐ 메인창이 클릭을 먹는 경우를 강제로 차단
            this.IsHitTestVisible = false;

            try
            {
                win.ShowActivated = true;
                win.Activate();
                win.ShowDialog();
            }
            finally
            {
                this.IsHitTestVisible = true;
                this.Activate();
            }
        }
    }
}