using BarryFileMan.Enums.Rename;

namespace BarryFileMan.Models.Config
{
    public class UserConfig
    {
        public UserGeneralConfig General { get; set; }

        public UserRenameConfig Rename { get; set; }

        public UserConfig() : this(new(), new()) { }
        public UserConfig(UserGeneralConfig general, UserRenameConfig rename) 
        {
            General = general;
            Rename = rename;
        }
    }
}
