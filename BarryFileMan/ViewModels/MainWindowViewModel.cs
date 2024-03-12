using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using BarryFileMan.ViewModels.About;
using BarryFileMan.ViewModels.Flatten;
using BarryFileMan.ViewModels.Rename;
using BarryFileMan.ViewModels.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace BarryFileMan.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly AboutViewModel _aboutViewModel = new();
        private readonly FlattenViewModel _flattenViewModel = new();
        private readonly RenameViewModel _renameViewModel = new();
        private readonly SettingsViewModel _settingsViewModel = new();

        public static string? AppName => AppManager.AppName;

        [ObservableProperty]
        private object? _viewContent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToolPaneExpandIcon))]
        private bool _toolPaneOpen = true;

        public string ToolPaneExpandIcon => ToolPaneOpen ? "ArrowExpandLeft" : "ArrowExpandRight";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Title))]
        private string _selectedToolPaneItem = "rename";

        public string Title
        {
            get
            {
                return SelectedToolPaneItem switch
                {
                    "about" => Resources.Resources.About,
                    "flatten" => Resources.Resources.Flatten,
                    "rename" => Resources.Resources.Rename,
                    "settings" => Resources.Resources.Settings,
                    _ => string.Empty,
                };
            }
        }

        [RelayCommand]
        private void ToggleToolPane()
        {
            ToolPaneOpen = !ToolPaneOpen;
        }

        [RelayCommand]
        private void ToolPaneItemSelected(string item)
        {
            if (SelectedToolPaneItem != item)
            {
                SelectedToolPaneItem = item;
                ChangeViewContent(item);
            }
            else
            {
                // Doing this to keep the UI in sync
                OnPropertyChanged(nameof(SelectedToolPaneItem));
            }
        }

        [RelayCommand]
        private static void ShowHelp()
        {
            AppManager.HelpWindowShowAsync();
        }

        public MainWindowViewModel()
        {
            ChangeViewContent(SelectedToolPaneItem);
            ToolPaneOpen = AppManager.UserConfig.Config.General.SidebarExpandedDefault;
            AppManager.UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);
        }

        private void OnUserConfigChanged(UserConfig userConfig)
        {
            ToolPaneOpen = AppManager.UserConfig.Config.General.SidebarExpandedDefault;
        }

        private void ChangeViewContent(string tool)
        {
            ViewContent = tool switch
            {
                "about" => _aboutViewModel,
                "flatten" => _flattenViewModel,
                "rename" => _renameViewModel,
                "settings" => _settingsViewModel,
                _ => null,
            };
        }
    }
}