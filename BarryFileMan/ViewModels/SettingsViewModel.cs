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

        public IEnumerable<ItemViewModel<RenameLoadOption>> RenameLoadOptions => new List<ItemViewModel<RenameLoadOption>>()
        {
            new ItemViewModel<RenameLoadOption>(RenameLoadOption.Files, "Files", 
                    AppManager.UserConfig.Config.DefaultRenameLoadOption == RenameLoadOption.Files)
            {
                Command = ConfigPropertyChangedCommand,
                CommandParameter = new ConfigPropertyChangedParam(
                    nameof(AppManager.UserConfig.Config.DefaultRenameLoadOption), RenameLoadOption.Files)
            },
            new ItemViewModel<RenameLoadOption>(RenameLoadOption.Folders, "Folders", 
                    AppManager.UserConfig.Config.DefaultRenameLoadOption == RenameLoadOption.Folders)
            {
                Command = ConfigPropertyChangedCommand,
                CommandParameter = new ConfigPropertyChangedParam(
                    nameof(AppManager.UserConfig.Config.DefaultRenameLoadOption), RenameLoadOption.Folders)
            }
        };

        [RelayCommand]
        private static async Task ThemeChanged(Theme theme)
        {
            ThemeHelper.ChangeTheme(theme);

            if (AppManager.UserConfig.Config.Theme != theme)
            {
                AppManager.UserConfig.Config.Theme = theme;
                await AppManager.UserConfig.SetConfigAsync(AppManager.UserConfig.Config);
            }
        }

        [RelayCommand]
        private static async Task ConfigPropertyChanged(ConfigPropertyChangedParam param)
        {
            var property = AppManager.UserConfig.Config.GetType().GetProperty(param.PropertyName);
            if(property != null)
            {
                property.SetValue(AppManager.UserConfig.Config, param.PropertyValue);
                await AppManager.UserConfig.SetConfigAsync(AppManager.UserConfig.Config);
            }
        }
    }

    public class ConfigPropertyChangedParam
    {
        public string PropertyName { get; set;}
        public object? PropertyValue { get; set; }

        public ConfigPropertyChangedParam(string propertyName, object? propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }
}
