using Avalonia;
using Avalonia.Styling;
using BarryFileMan.Enums.Config;

namespace BarryFileMan.Helpers
{
    public static class ThemeHelper
    {
        public static void ChangeTheme(Theme theme)
        {
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = GetThemeVariant(theme);
            }
        }

        private static ThemeVariant GetThemeVariant(Theme theme)
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
