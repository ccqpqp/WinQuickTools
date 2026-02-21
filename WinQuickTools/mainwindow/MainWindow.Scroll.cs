using System;
using System.Windows;
using System.Windows.Input;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        private void OnWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (!_hasTarget)
            {
                _targetOffset = MainScroll.VerticalOffset;
                _hasTarget = true;
            }

            // WPF: 휠 위(Delta>0) = 위로. 오프셋은 줄어야 위로 가므로 -Delta.
            _targetOffset -= e.Delta * WheelScale;

            if (_targetOffset < 0) _targetOffset = 0;
            if (_targetOffset > MainScroll.ScrollableHeight) _targetOffset = MainScroll.ScrollableHeight;
        }

        private void OnRender(object? sender, EventArgs e)
        {
            if (!_hasTarget) return;

            long now = DateTime.UtcNow.Ticks;
            if (_lastTicks == 0) _lastTicks = now;

            double dt = (now - _lastTicks) / (double)TimeSpan.TicksPerSecond;
            _lastTicks = now;

            // dt 보호
            if (dt <= 0) dt = 1.0 / 60.0;
            if (dt > 0.05) dt = 0.05;

            double current = MainScroll.VerticalOffset;
            double diff = _targetOffset - current;

            // 거의 도착하면 스냅
            if (Math.Abs(diff) < 0.35)
            {
                MainScroll.ScrollToVerticalOffset(_targetOffset);
                _hasTarget = false;
                return;
            }

            // 지수형 보간: 튐 방지 + 자연스러움
            double step = diff * (1.0 - Math.Exp(-SmoothStrength * dt));
            MainScroll.ScrollToVerticalOffset(current + step);
        }
    }
}