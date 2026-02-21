using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Win32;
using Application = System.Windows.Application;
using WinQuickTools.Features;
using WinQuickTools.Services;
using WinQuickTools.Windows;
using System.Diagnostics;

namespace WinQuickTools

{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon? _trayIcon;
        private bool _allowExit;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // 고정 6슬롯 만들기(자리 유지용)
            for (int i = 0; i < 6; i++) FavoriteSlots.Add(null);

            // =====================================================
            // ⭐ 기본 핵심 기능 (Top Tier)
            // =====================================================
            _allItems.Add(new FeatureItem("ext", "확장자 표시", "파일 뒤 .txt .jpg 표시"));
            _allItems.Add(new FeatureItem("hidden", "숨김 파일 보기", "숨겨진 파일 표시"));
            _allItems.Add(new FeatureItem("restart", "탐색기 다시 시작", "먹통·오류 즉시 해결"));
            _allItems.Add(new FeatureItem("copypath", "경로 복사", "파일 위치 한 번에 복사"));
            _allItems.Add(new FeatureItem("quickcapture", "빠른 캡처 버튼", "앱 실행 중 F1단축키로 변경"));
            _allItems.Add(new FeatureItem("restoreall", "설정 원래대로", "모든 변경 복구"));

            // =====================================================
            // ⚡ 문제 해결 (가끔 쓰지만 필수)
            // =====================================================
            _allItems.Add(new FeatureItem("dnsflush", "인터넷 문제 해결", "네트워크 캐시 초기화"));
            _allItems.Add(new FeatureItem("clipboardhistory", "복사 여러개 사용", "Win+V 기록 활성화"));
            _allItems.Add(new FeatureItem("contextmenu", "우클릭 메뉴 사용", "shfit + 우클릭에 WinQuickTools 추가"));

            // =====================================================
            // 🧰 편의 설정 (찾기 어려운 설정 모음)
            // =====================================================
            _allItems.Add(new FeatureItem("downloadsorganize", "다운로드 폴더 정리", "파일 자동 분류"));
            _allItems.Add(new FeatureItem("screenshotpath", "스크린샷 저장 위치 변경", "저장 폴더 변경"));

            // =====================================================
            // 🖱️ 우클릭 기능
            // =====================================================
            RightClickItems.Add(new FeatureItem("exportlist", "파일 목록 만들기", "폴더 내용 텍스트 생성"));
            RightClickItems.Add(new FeatureItem("renamebatch", "파일 이름 정리", "번호·공백 자동 정리"));
            RightClickItems.Add(new FeatureItem("renumber", "번호 자동 붙이기", "001,002 순서 생성"));
            RightClickItems.Add(new FeatureItem("fullpathcopy", "파일 경로 복사", "파일 포함 전체 경로"));

            // ⭐ 저장값 있으면 로드 / 없을 때만 기본값 적용
            if (!LoadFavorites()) InitDefaultFavorites();

            RebuildNonFavorites();
            UpdateStatusFromSystem();

            // ⭐ 우클릭 메뉴 상태 읽기
            _contextEnabled =
                Registry.CurrentUser.OpenSubKey(@"Software\Classes\*\shell\WinQuickTools") != null;
            UpdateContextStatus();

            // ✅ Loaded 에서만 무거운/외부 작업 시작 (여기서부터가 핵심)
            Loaded += MainWindow_Loaded;

        }



        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainScroll.PreviewMouseWheel += OnWheel;
            CompositionTarget.Rendering += OnRender;

            // ✅ 창이 먼저 그려지게 한 프레임 뒤에 실행
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try { InitTray(); }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.ToString(), "Tray 오류"); }

                // ✅ UI 안 막히게 별도 STA 스레드에서 실행
                RunSta(() =>
                {
                    try { ContextMenuInstaller.Install(); }
                    catch (Exception ex)
                    {
                        // UI로 에러 띄우기
                        Dispatcher.Invoke(() =>
                            System.Windows.MessageBox.Show(ex.ToString(), "ContextMenuInstaller 오류"));
                    }
                });

            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private static void RunSta(Action action)
        {
            var t = new System.Threading.Thread(() => action());
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }


        private void InitTray()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = GetExeIcon(),
                Visible = true,
                Text = "WinQuickTools"
            };

            var menu = new ContextMenuStrip();

            menu.Items.Add("열기", null, (_, __) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                });
            });

            menu.Items.Add("종료", null, (_, __) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _allowExit = true;
                    _trayIcon!.Visible = false;
                    Close();
                });
            });

            _trayIcon.ContextMenuStrip = menu;

            _trayIcon.DoubleClick += (_, __) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                });
            };
        }

        private static System.Drawing.Icon GetExeIcon()
        {
            // single-file에서도 안전하게 exe 아이콘 가져오기
            using var p = Process.GetCurrentProcess();
            var exePath = p.MainModule!.FileName!;
            return System.Drawing.Icon.ExtractAssociatedIcon(exePath) ?? System.Drawing.SystemIcons.Application;
        }


        // =========================
        // 버튼 핸들러들
        // =========================
        private void ExportList_Click(object sender, RoutedEventArgs e) => FileFeatures.ExportList();
        private void RenameBatch_Click(object sender, RoutedEventArgs e) => FileFeatures.RenameBatch();
        private void Renumber_Click(object sender, RoutedEventArgs e) => FileFeatures.Renumber();
        private void FullPathCopy_Click(object sender, RoutedEventArgs e) => FileFeatures.FullPathCopy();

        private void DnsFlush_Click(object sender, RoutedEventArgs e) => SystemFeatures.DnsFlush();
        private void ClipboardHistory_Click(object sender, RoutedEventArgs e) => SystemFeatures.ToggleClipboardHistory();
        private void ContextMenu_Click(object sender, RoutedEventArgs e) => SystemFeatures.EnableFullContextMenu();
        private void DownloadsOrganize_Click(object sender, RoutedEventArgs e) => SystemFeatures.OrganizeDownloads();
        private void ScreenshotPath_Click(object sender, RoutedEventArgs e) => SystemFeatures.ChangeScreenshotPath();

    }
}