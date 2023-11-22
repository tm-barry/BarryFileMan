using BarryFileMan.Enums.Config;

namespace BarryFileMan.Models.Config
{
    public class UserGeneralConfig
    {
        public Theme Theme { get; set; }

        public UserGeneralConfig() : this(Theme.Dark) { }
        public UserGeneralConfig(Theme theme)
        {
            Theme = theme;
        }
    }
}
