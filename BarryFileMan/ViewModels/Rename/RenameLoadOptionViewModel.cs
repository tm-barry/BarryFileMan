using BarryFileMan.Enums.Rename;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace BarryFileMan.ViewModels.Rename
{
    public class RenameLoadOptionViewModel : ObservableObject
    {
        public RenameLoadOption Type { get; private set; }
        public string DisplayText { get; private set; }
        public string Icon { get; private set; }
        public ICommand Command { get; private set; }

        public RenameLoadOptionViewModel(RenameLoadOption type, string displayText, string icon, ICommand command)
        {
            Type = type;
            DisplayText = displayText;
            Icon = icon;
            Command = command;
        }
    }
}
