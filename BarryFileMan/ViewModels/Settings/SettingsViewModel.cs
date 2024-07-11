using BarryFileMan.Managers;
using BarryFileMan.Views.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private async Task CopyText(string text)
        {
            await AppManager.CopyText(text);
        }

        [RelayCommand]
        private async Task RevertToDefault()
        {
            IsBusy = true;
            try
            {
                var result = await AppManager.MsgBoxShowWindowDialogAsync(
                    Resources.Resources.RevertToDefault, 
                    Resources.Resources.RevertToDefaultConfirmationMessage, 
                    MsgBoxButtons.YesNo, 
                    MsgBoxIcons.Question);

                if(result.result == MsgBoxResult.Yes)
                {
                    var config = await AppManager.UserConfig.SetConfigAsync(new());

                    if(config == null)
                    {
                        await AppManager.MsgBoxShowWindowDialogAsync(
                            Resources.Resources.Error, Resources.Resources.RevertToDefaultErrorMessage, MsgBoxButtons.Ok, MsgBoxIcons.Error);
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
                        Resources.Resources.Error, $"{ex.Message}\n{ex.InnerException?.Message}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
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
                        Resources.Resources.Error, $"{ex.Message}\n{ex.InnerException?.Message}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
            }
        }

        private bool CanSaveChanges()
        {
            return Config.IsDirty;
        }
    }
}
