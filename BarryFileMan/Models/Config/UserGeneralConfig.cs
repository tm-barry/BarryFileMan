using BarryFileMan.Enums.Config;

namespace BarryFileMan.Models.Config
{
    public class UserGeneralConfig
    {
        public Theme Theme { get; set; }

        public bool SidebarExpandedDefault { get; set; }

        public UserGeneralConfig() : this(Theme.Dark, true) { }
        public UserGeneralConfig(Theme theme, bool sideBarExpandedDefault)
        {
            Theme = theme;
            SidebarExpandedDefault = sideBarExpandedDefault;
        }
    }
}
