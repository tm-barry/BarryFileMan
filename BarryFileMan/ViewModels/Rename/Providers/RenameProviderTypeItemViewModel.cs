using BarryFileMan.Rename.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RenameProviderTypeItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private RenameProviderTypes _type;

        [ObservableProperty]
        private string _display;

        [ObservableProperty]
        private string _icon;

        [ObservableProperty]
        private bool _isImage;

        public RenameProviderTypeItemViewModel(RenameProviderTypes type, string display, string icon, bool isImage = false)
        {
            Type = type;
            Display = display;
            Icon = icon;
            IsImage = isImage;
        }
    }
}
