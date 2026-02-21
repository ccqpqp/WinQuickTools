using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WinQuickTools.Services
{
    public static class ToastService
    {
        public static void Show(string title, string message, bool isOn)
        {
            var toast = new Window
            {
                Width = 260,
                Height = 80,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                ShowInTaskbar = false,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SnapsToDevicePixels = true
            };
            var accent = isOn
     ? Color.FromRgb(80, 200, 140)   // 부드러운 초록
     : Color.FromRgb(140, 140, 140); // 뉴트럴 회색

            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(235, 28, 28, 30)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(14),
                BorderBrush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                BorderThickness = new Thickness(1),
            };

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            // ✅ 상태 점
            var dot = new Border
            {
                Width = 8,
                Height = 8,
                CornerRadius = new CornerRadius(4),
                Background = new SolidColorBrush(accent),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 2, 10, 0),
                SnapsToDevicePixels = true
            };

            Grid.SetColumn(dot, 0);
            grid.Children.Add(dot);


            // ✅ 텍스트 영역
            var textPanel = new Grid();
            textPanel.RowDefinitions.Add(new RowDefinition());
            textPanel.RowDefinitions.Add(new RowDefinition());

            var titleText = new TextBlock
            {
                Text = title,
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                TextWrapping = TextWrapping.NoWrap
            };

            var messageText = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                FontSize = 12,
                Margin = new Thickness(0, 2, 0, 0),
                TextWrapping = TextWrapping.NoWrap
            };

            Grid.SetRow(titleText, 0);
            Grid.SetRow(messageText, 1);

            textPanel.Children.Add(titleText);
            textPanel.Children.Add(messageText);

            Grid.SetColumn(textPanel, 1);
            grid.Children.Add(textPanel);

            border.Child = grid;
            toast.Content = border;

            toast.Left = SystemParameters.WorkArea.Right - 280;
            toast.Top = SystemParameters.WorkArea.Bottom - 100;

            toast.Show();

            toast.Dispatcher.InvokeAsync(() =>
            {
                toast.Clip = new RectangleGeometry(
                    new Rect(0, 0, toast.ActualWidth, toast.ActualHeight),
                    12, 12); // border CornerRadius랑 동일
            });


            toast.Loaded += (_, __) =>
            {
                toast.Clip = new RectangleGeometry(
                    new Rect(0, 0, toast.ActualWidth, toast.ActualHeight),
                    12, 12); // CornerRadius와 동일
            };

            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                toast.Dispatcher.Invoke(() => toast.Close());
            });
        }
    }
}