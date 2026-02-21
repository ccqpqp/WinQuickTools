using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinQuickTools

{
    public sealed class FeatureItem : INotifyPropertyChanged
    {
        public string Id { get; }
        public string Title { get; }
        public string SubTitle { get; }

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }

        public FeatureItem(string id, string title, string subTitle)
        {
            Id = id;
            Title = title;
            SubTitle = subTitle;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}