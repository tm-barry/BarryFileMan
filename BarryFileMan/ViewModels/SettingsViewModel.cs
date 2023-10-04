using BarryFileMan.Helpers;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public IEnumerable<ItemViewModel<Theme>> Themes => new List<ItemViewModel<Theme>>()
        {
            new ItemViewModel<Theme>(Theme.Default, "System Default", AppManager.UserConfig.Config.Theme == Theme.Default)
            {
                Command = ThemeChangedCommand,
                CommandParameter = Theme.Default
            },
            new ItemViewModel<Theme>(Theme.Dark, "Dark", AppManager.UserConfig.Config.Theme == Theme.Dark)
            {
                Command = ThemeChangedCommand,
                CommandParameter = Theme.Dark
            },
            new ItemViewModel<Theme>(Theme.Light, "Light", AppManager.UserConfig.Config.Theme == Theme.Light)
            {
                Command = ThemeChangedCommand,
                CommandParameter = Theme.Light
            },
        };

        [RelayCommand]
        private async Task ThemeChanged(Theme theme)
        {
            ThemeHelper.ChangeTheme(theme);
            await AppManager.UserConfig.UpdateThemeAsync(theme);
        }
    }
}
