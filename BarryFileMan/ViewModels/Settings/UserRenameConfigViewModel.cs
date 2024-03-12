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

        public RenameLoadOption SelectedDefaultLoadOption => RenameLoadOptions.FirstOrDefault((opt) => opt.Selected)?.Item ?? RenameLoadOption.Files;

        public bool IsDirty => SelectedDefaultLoadOption != _config.DefaultLoadOption;

        public UserRenameConfigViewModel(UserRenameConfig config)
        {
            _config = config;
            RenameLoadOptions.CollectionChanged += RenameLoadOptions_CollectionChanged;
            ApplyConfig();
        }

        private void RenameLoadOptions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
                foreach (ItemViewModel<RenameLoadOption> item in e.NewItems)
                    item.PropertyChanged += RenameLoadOption_PropertyChanged;

            if (e?.OldItems != null)
                foreach (ItemViewModel<RenameLoadOption> item in e.OldItems)
                    item.PropertyChanged -= RenameLoadOption_PropertyChanged;

            OnPropertyChanged(nameof(SelectedDefaultLoadOption));
            OnPropertyChanged(nameof(IsDirty));
        }

        private void RenameLoadOption_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedDefaultLoadOption));
            OnPropertyChanged(nameof(IsDirty));
        }

        private void ApplyConfig()
        {
            RenameLoadOptions.Clear();
            RenameLoadOptions.Add(new ItemViewModel<RenameLoadOption>(RenameLoadOption.Files, Resources.Resources.Files, _config.DefaultLoadOption == RenameLoadOption.Files));
            RenameLoadOptions.Add(new ItemViewModel<RenameLoadOption>(RenameLoadOption.Folders, Resources.Resources.Folders, _config.DefaultLoadOption == RenameLoadOption.Folders));
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
