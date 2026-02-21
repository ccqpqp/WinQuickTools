// ✅ 이 파일 전체 교체

using WinQuickTools.Services;
using System.Threading.Tasks;

namespace WinQuickTools
{
    public partial class MainWindow
    {
        private bool _contextEnabled;
        private bool _isTogglingContext;

        private async void ToggleContextMenu()
        {
            if (_isTogglingContext) return;

            try
            {
                _isTogglingContext = true;

                if (ContextMenuRegistrar.IsEnabled())
                    ContextMenuRegistrar.Disable();
                else
                    ContextMenuRegistrar.Enable();

                // ⭐ explorer registry sync 기다림
                await Task.Delay(250);

                // ⭐ 실제 상태 다시 읽기 (핵심)
                _contextEnabled = ContextMenuRegistrar.IsEnabled();

                UpdateContextStatus();

                ToastService.Show(
                    "우클릭 메뉴",
                    _contextEnabled ? "켜짐" : "꺼짐",
                    _contextEnabled);
            }
            finally
            {
                _isTogglingContext = false;
            }
        }

        private void UpdateContextStatus()
        {
            var item = FindItem("contextmenu");
            if (item != null)
                item.StatusText = _contextEnabled ? "현재: 켜짐" : "현재: 꺼짐";
        }
    }
}