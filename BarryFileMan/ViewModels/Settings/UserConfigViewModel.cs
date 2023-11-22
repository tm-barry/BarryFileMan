using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserConfigViewModel : BaseUserConfigViewModel<UserConfig>
    {
        [ObservableProperty]
        private UserGeneralConfigViewModel _general;

        [ObservableProperty]
        private UserRenameConfigViewModel _rename;

        public bool IsDirty => General.IsDirty || Rename.IsDirty;

        public UserConfigViewModel(UserConfig config)
        {
            General = new(config.General);
            Rename = new(config.Rename);
            General.PropertyChanged += General_PropertyChanged;
            Rename.PropertyChanged += Rename_PropertyChanged;
        }

        ~UserConfigViewModel()
        {
            General.PropertyChanged -= General_PropertyChanged;
            Rename.PropertyChanged -= Rename_PropertyChanged;
        }

        private void General_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(General.IsDirty))
                OnPropertyChanged(nameof(IsDirty));
        }

        private void Rename_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Rename.IsDirty))
                OnPropertyChanged(nameof(IsDirty));
        }

        public override UserConfig UndoChanges()
        {
            return new(General.UndoChanges(), Rename.UndoChanges());
        }

        public override UserConfig ApplyChanges()
        {
            return new(General.ApplyChanges(), Rename.ApplyChanges());
        }
    }
}
