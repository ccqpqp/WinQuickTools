using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace WinQuickTools
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<FeatureItem> _allItems = new();
        public ObservableCollection<FeatureItem?> FavoriteSlots { get; } = new();

        public ObservableCollection<FeatureItem> NonFavoriteItems { get; } = new();
        public ObservableCollection<FeatureItem> RightClickItems { get; } = new();

        private bool _f1CaptureEnabled = false;
        private const int HOTKEY_ID = 1;

        // ---------- Smooth scroll ----------
        private double _targetOffset;
        private bool _hasTarget;
        private long _lastTicks;

        // 감도/부드러움 튜닝 값
        private const double WheelScale = 0.35;     // 작을수록 덜 튐
        private const double SmoothStrength = 14.0; // 클수록 더 빨리 따라감 (과하면 튐)

        private readonly string _favPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WinQuickTools",
                "favorites.json");

        // HotKey handle
        private IntPtr _windowHandle;
    }
}