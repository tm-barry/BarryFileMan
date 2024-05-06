namespace BarryFileMan.Models.Config
{
    public class UserConfig
    {
        public UserFlattenConfig Flatten { get; set; }

        public UserGeneralConfig General { get; set; }

        public UserRenameConfig Rename { get; set; }

        public UserTMDBConfig Tmdb { get; set; }

        public UserConfig() : this(new(), new(), new(), new()) { }
        public UserConfig(UserFlattenConfig flatten, UserGeneralConfig general, UserRenameConfig rename, UserTMDBConfig tmdb) 
        {
            Flatten = flatten;
            General = general;
            Rename = rename;
            Tmdb = tmdb;
        }
    }
}
