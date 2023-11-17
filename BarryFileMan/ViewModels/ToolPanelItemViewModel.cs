using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BarryFileMan.ViewModels
{
    public partial class ToolPanelItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _text;

        [ObservableProperty]
        private string? _icon;

        [ObservableProperty]
        private bool? _selected;

        [ObservableProperty]
        private ICommand? _command;

        [ObservableProperty]
        private string? _commandParameter;
    }
}
