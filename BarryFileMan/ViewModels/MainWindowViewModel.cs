
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using BarryFileMan.ViewModels.Comics;
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
        private readonly ComicsViewModel _comicsViewModel = new();
        private readonly FlattenViewModel _flattenViewModel = new();
        private readonly RenameViewModel _renameViewModel = new();
        private readonly SettingsViewModel _settingsViewModel = new();

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
                    "comics" => "Comics",
                    "flatten" => "Flatten",
                    "rename" => "Rename",
                    "settings" => "Settings",
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
                "comics" => _comicsViewModel,
                "flatten" => _flattenViewModel,
                "rename" => _renameViewModel,
                "settings" => _settingsViewModel,
                _ => null,
            };
        }
    }
}