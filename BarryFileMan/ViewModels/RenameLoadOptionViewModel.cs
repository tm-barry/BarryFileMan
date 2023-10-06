using System.Windows.Input;

namespace BarryFileMan.ViewModels
{
    public class RenameLoadOptionViewModel : ViewModelBase
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

    public enum RenameLoadOption
    {
        Files,
        Folders
    }
}
