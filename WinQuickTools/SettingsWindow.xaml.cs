using System.Windows;
using System.Windows.Input;
using WinQuickTools.Features;
using WinQuickTools.Services;
using WinQuickTools.Windows;

namespace WinQuickTools
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            AutoStartCheck.IsChecked = AutostartService.IsEnabled();
        }

        private void AutoStart_Checked(object sender, RoutedEventArgs e)
        {
            AutostartService.SetEnabled(true);
            HintText.Text = "자동 실행: 켜짐";
        }

        private void AutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            AutostartService.SetEnabled(false);
            HintText.Text = "자동 실행: 꺼짐";
        }

        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            // EtDialog 표준 팝업
            if (EtDialog.Confirm("설정 전체 초기화", "모든 설정을 초기 상태로 되돌릴까요?") != true)
                return;

            // 네가 만들어둔 전체 초기화 호출로 연결
            // (SystemRestore.RestoreAll() 또는 네 RestoreAll_Click 로직으로 맞춰)
            SystemRestore.RestoreAll();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}