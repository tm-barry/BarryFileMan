using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserConfigViewModel : BaseUserConfigViewModel<UserConfig>
    {
        [ObservableProperty]
        private UserFlattenConfigViewModel _flatten;

        [ObservableProperty]
        private UserGeneralConfigViewModel _general;

        [ObservableProperty]
        private UserRenameConfigViewModel _rename;

        public override bool IsDirty => Flatten.IsDirty || General.IsDirty || Rename.IsDirty;

        public UserConfigViewModel(UserConfig config)
        {
            Flatten = new(config.Flatten);
            General = new(config.General);
            Rename = new(config.Rename);

            Flatten.PropertyChanged += UserConfig_PropertyChanged;
            General.PropertyChanged += UserConfig_PropertyChanged;
            Rename.PropertyChanged += UserConfig_PropertyChanged;
        }

        ~UserConfigViewModel()
        {
            Flatten.PropertyChanged -= UserConfig_PropertyChanged;
            General.PropertyChanged -= UserConfig_PropertyChanged;
            Rename.PropertyChanged -= UserConfig_PropertyChanged;
        }

        private void UserConfig_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDirty))
                OnPropertyChanged(nameof(IsDirty));
        }

        public override UserConfig UndoChanges()
        {
            return new(Flatten.UndoChanges(), General.UndoChanges(), Rename.UndoChanges());
        }

        public override UserConfig ApplyChanges()
        {
            return new(Flatten.ApplyChanges(), General.ApplyChanges(), Rename.ApplyChanges());
        }
    }
}
