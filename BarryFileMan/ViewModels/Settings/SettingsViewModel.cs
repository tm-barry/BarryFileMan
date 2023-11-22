using BarryFileMan.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserConfigViewModel _config;
        partial void OnConfigChanged(UserConfigViewModel? oldValue, UserConfigViewModel newValue)
        {
            if(oldValue != null)
            {
                oldValue.PropertyChanged -= Config_PropertyChanged;
            }

            if(newValue != null)
            {
                newValue.PropertyChanged += Config_PropertyChanged;
            }
        }
        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Config.IsDirty))
            {
                UndoChangesCommand.NotifyCanExecuteChanged();
                SaveChangesCommand.NotifyCanExecuteChanged();
            }
        }

        [ObservableProperty]
        private bool _isBusy;

        public SettingsViewModel()
        {
            Config = new(AppManager.UserConfig.Config);
        }

        [RelayCommand]
        private async Task RevertToDefault()
        {
            IsBusy = true;
            try
            {
                var result = await AppManager.MsgBoxShowWindowDialogAsync(
                    "Revert to Default", "This will revert all settings to the application's defaults. Are you sure you want to continue?", ButtonEnum.YesNo, Icon.Question);

                if(result == ButtonResult.Yes)
                {
                    var config = await AppManager.UserConfig.SetConfigAsync(new());

                    if(config == null)
                    {
                        await AppManager.MsgBoxShowWindowDialogAsync(
                            "Error", "Something went wrong reverting to default settings.", ButtonEnum.Ok, Icon.Error);
                    }
                    else
                    {
                        Config = new(config);
                    }
                }
            }
            catch(Exception ex)
            {
                await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"{ex.Message}\n{ex.InnerException?.Message}", ButtonEnum.Ok, Icon.Error);
            }

            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(CanUndoChanges))]
        private void UndoChanges()
        {
            Config.UndoChanges();
        }

        private bool CanUndoChanges()
        {
            return Config.IsDirty;
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            try
            {
                var config = Config.ApplyChanges();
                await AppManager.UserConfig.SetConfigAsync(config);
            }
            catch (Exception ex)
            {
                await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"{ex.Message}\n{ex.InnerException?.Message}", ButtonEnum.Ok, Icon.Error);
            }
        }

        private bool CanSaveChanges()
        {
            return Config.IsDirty;
        }
    }
}
