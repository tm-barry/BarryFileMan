using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using BarryFileMan.Helpers;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private static string _menuItemSelectedIcon = "CheckBold";

        public ObservableCollection<MenuItemViewModel> MenuItems => new()
        {
            new MenuItemViewModel()
            {
                Header = "File",
                Items = new[]
                {
                    new MenuItemViewModel()
                    {
                        Header = "Theme",
                        Icon = "ThemeLightDark",
                        Items = new[]
                        {
                            new MenuItemViewModel()
                            {
                                Header = "Default",
                                Command = ThemeChangedCommand,
                                CommandParameter = Theme.Default,
                                Icon = AppManager.UserConfig.Config.Theme == Theme.Default
                                    ? _menuItemSelectedIcon
                                    : null,
                            },
                            new MenuItemViewModel()
                            {
                                Header = "Dark",
                                Command = ThemeChangedCommand,
                                CommandParameter = Theme.Dark,
                                Icon = AppManager.UserConfig.Config.Theme == Theme.Dark
                                    ? _menuItemSelectedIcon
                                    : null,
                            },
                            new MenuItemViewModel()
                            {
                                Header = "Light",
                                Command = ThemeChangedCommand,
                                CommandParameter = Theme.Light,
                                Icon = AppManager.UserConfig.Config.Theme == Theme.Light
                                    ? _menuItemSelectedIcon
                                    : null,
                            }
                        }
                    }
                }
            }
        };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToolPaneExpandIcon))]
        private bool _toolPaneOpen = true;

        public string ToolPaneExpandIcon => ToolPaneOpen ? "ArrowExpandLeft" : "ArrowExpandRight";

        public MainWindowViewModel() { }

        [RelayCommand]
        private async Task ThemeChangedAsync(Theme theme)
        {
            var config = AppManager.UserConfig.Config;
            config.Theme = theme;
            await AppManager.UserConfig.SetConfigAsync(config);

            var themeMenuItem = MenuItems.FirstOrDefault((item) => item.Header == "Theme");
            if (themeMenuItem?.Items != null)
            {
                foreach (var item in themeMenuItem.Items)
                {
                    item.Icon = item.CommandParameter != null
                        ? (Theme)item.CommandParameter == theme
                            ? _menuItemSelectedIcon
                            : null
                        : null;
                }
            }
            OnPropertyChanged(nameof(MenuItems));

            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = ThemeHelper.GetThemeVariant(theme);
            }
        }

        [RelayCommand]
        private void ToggleToolPane()
        {
            ToolPaneOpen = !ToolPaneOpen;
        }
    }
}