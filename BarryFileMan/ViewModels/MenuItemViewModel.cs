using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;

namespace BarryFileMan.ViewModels
{
    public partial class MenuItemViewModel : ViewModelBase
    {
        public string? Header { get; set; }
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }
        public IList<MenuItemViewModel>? Items { get; set; }

        [ObservableProperty]
        private string? _icon;
    }
}
