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
        private bool _selected;

        [ObservableProperty]
        private ICommand? _command;

        [ObservableProperty]
        private object? _commandParameter;

        public ItemViewModel(T item, string display, bool selected)
        {
            Item = item;
            Display = display;
            Selected = selected;
        }
    }
}
