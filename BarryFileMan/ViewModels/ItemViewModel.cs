using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BarryFileMan.ViewModels
{
    public partial class ItemViewModel<T> : ObservableObject
    {
        [ObservableProperty]
        private T _item;

        [ObservableProperty]
        private string _display;

        [ObservableProperty]
        private string? _icon;

        [ObservableProperty]
        private bool _selected;

        [ObservableProperty]
        private ICommand? _command;

        [ObservableProperty]
        private object? _commandParameter;

        public ItemViewModel(T item, string display, bool selected, 
            string? icon = null, ICommand? command = null, object? commandParameter = null)
        {
            Item = item;
            Display = display;
            Selected = selected;
            Icon = icon;
            Command = command;
            CommandParameter = commandParameter;
        }
    }
}
