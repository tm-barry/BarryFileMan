using Avalonia.Styling;

namespace BarryFileMan.Models.Config
{
    public class UserConfig
    {
        public Theme Theme { get; set; }

        public UserConfig() : this(Theme.Default) { }

        public UserConfig(Theme theme) 
        {
            Theme = theme;
        }
    }

    public enum Theme
    {
        Default,
        Dark,
        Light
    }
}
