using BarryFileMan.Enums.Rename;

namespace BarryFileMan.Models.Config
{
    public class UserRenameConfig
    {
        public RenameLoadOption DefaultLoadOption { get; set; }

        public UserRenameConfig() : this(RenameLoadOption.Files) { }
        public UserRenameConfig(RenameLoadOption defaultLoadOption)
        {
            DefaultLoadOption = defaultLoadOption;
        }
    }
}
