using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private SettingsViewModel _settingsViewModel = new();

        [ObservableProperty]
        private object? _viewContent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToolPaneExpandIcon))]
        private bool _toolPaneOpen = true;

        public string ToolPaneExpandIcon => ToolPaneOpen ? "ArrowExpandLeft" : "ArrowExpandRight";

        [ObservableProperty]
        private string _selectedToolPaneItem = "rename";

        [RelayCommand]
        private void ToggleToolPane()
        {
            ToolPaneOpen = !ToolPaneOpen;
        }

        [RelayCommand]
        private void ToolPaneItemSelected(string item)
        {
            SelectedToolPaneItem = item;
            ChangeViewContent(item);
        }

        public MainWindowViewModel()
        {
            ChangeViewContent(SelectedToolPaneItem);
        }

        private void ChangeViewContent(string tool)
        {
            switch(tool)
            {
                case "settings":
                    ViewContent = _settingsViewModel;
                    break;
                case "rename":
                case "flatten":
                case "comics":
                default:
                    ViewContent = null;
                    break;
            }
        }
    }
}