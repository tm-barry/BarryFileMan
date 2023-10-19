using BarryFileMan.Enums.Rename;
using BarryFileMan.ViewModels;

namespace BarryFileMan.Models.Config
{
    public class UserConfig
    {
        public Theme Theme { get; set; }

        public RenameLoadOption DefaultRenameLoadOption { get; set; }

        public UserConfig() : this(Theme.Default, RenameLoadOption.Files) { }

        public UserConfig(Theme theme, RenameLoadOption defaultRenameLoadOption) 
        {
            Theme = theme;
            DefaultRenameLoadOption = defaultRenameLoadOption;
        }
    }

    public enum Theme
    {
        Default,
        Dark,
        Light
    }
}
