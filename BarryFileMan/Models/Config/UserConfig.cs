namespace BarryFileMan.Models.Config
{
    public class UserConfig
    {
        public UserFlattenConfig Flatten { get; set; }

        public UserGeneralConfig General { get; set; }

        public UserRenameConfig Rename { get; set; }

        public UserConfig() : this(new(), new(), new()) { }
        public UserConfig(UserFlattenConfig flatten, UserGeneralConfig general, UserRenameConfig rename) 
        {
            Flatten = flatten;
            General = general;
            Rename = rename;
        }
    }
}
