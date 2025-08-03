using Avalonia.Media.Imaging;
using Avalonia.Platform;
using BarryFileMan.Rename.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RenameProviderTypeItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private RenameProviderTypes _type;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SettingsDisplay))]
        private string _display;

        public string SettingsDisplay => $"{Display} {Resources.Resources.Settings}";

        [ObservableProperty]
        private string _icon;
        
        [ObservableProperty]
        private Bitmap? _iconBitmap;

        [ObservableProperty]
        private int? _iconHeight;

        [ObservableProperty]
        private int? _iconWidth;

        [ObservableProperty]
        private bool _isImage;

        public RenameProviderTypeItemViewModel(RenameProviderTypes type, string display, string icon,
            int? iconHeight = null, int? iconWidth = null, bool isImage = false)
        {
            Type = type;
            Display = display;
            Icon = icon;
            IconHeight = iconHeight;
            IconWidth = iconWidth;
            IsImage = isImage;

            if (isImage && !string.IsNullOrWhiteSpace(icon))
            {
                var path = icon.TrimStart('/');
                var uri = new Uri($"avares://BarryFileMan/{path}");
                IconBitmap = new Bitmap(AssetLoader.Open(uri));
            }
        }
    }
}
