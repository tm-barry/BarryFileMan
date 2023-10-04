using Avalonia.Styling;
using BarryFileMan.Models.Config;

namespace BarryFileMan.Helpers
{
    public static class ThemeHelper
    {
        public static ThemeVariant GetThemeVariant(Theme theme)
        {
            return theme switch
            {
                Theme.Dark => ThemeVariant.Dark,
                Theme.Light => ThemeVariant.Light,
                _ => ThemeVariant.Default,
            };
        }
    }
}
