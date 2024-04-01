using BarryFileMan.Enums.Config;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserRenameConfigViewModel : BaseUserConfigViewModel<UserRenameConfig>
    {
        private UserRenameConfig _config;

        [ObservableProperty]
        private ObservableCollection<ItemViewModel<RenameLoadOption>> _renameLoadOptions = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedDefaultLoadOption))]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private int? _selectedDefaultLoadOptionIndex;

        public RenameLoadOption SelectedDefaultLoadOption => RenameLoadOptions.ElementAtOrDefault(SelectedDefaultLoadOptionIndex ?? -1)?.Item ?? RenameLoadOption.Files;

        public bool IsDirty => SelectedDefaultLoadOption != _config.DefaultLoadOption;

        public UserRenameConfigViewModel(UserRenameConfig config)
        {
            _config = config;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            RenameLoadOptions.Clear();
            RenameLoadOptions.Add(new ItemViewModel<RenameLoadOption>(RenameLoadOption.Files, Resources.Resources.Files, false, "FileMultiple"));
            RenameLoadOptions.Add(new ItemViewModel<RenameLoadOption>(RenameLoadOption.Folders, Resources.Resources.Folders, false, "FolderMultiple"));

            var selectedRenameLoadOption = RenameLoadOptions.FirstOrDefault((option) => option.Item == _config.DefaultLoadOption);
            SelectedDefaultLoadOptionIndex = selectedRenameLoadOption != null ? RenameLoadOptions.IndexOf(selectedRenameLoadOption) : -1;
        }

        public override UserRenameConfig UndoChanges()
        {
            ApplyConfig();
            return _config;
        }

        public override UserRenameConfig ApplyChanges()
        {
            _config = new(SelectedDefaultLoadOption);
            OnPropertyChanged(nameof(SelectedDefaultLoadOption));
            OnPropertyChanged(nameof(IsDirty));
            return _config;
        }
    }
}
