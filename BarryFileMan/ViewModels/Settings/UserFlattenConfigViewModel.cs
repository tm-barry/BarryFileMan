using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserFlattenConfigViewModel : BaseUserConfigViewModel<UserFlattenConfig>
    {
        private UserFlattenConfig _config;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private string? _defaultRegexFileFilter;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private bool _deleteExcludedFilesDefault;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private bool _deleteEmptyFoldersDefault;

        public override bool IsDirty => DefaultRegexFileFilter != _config.DefaultRegexFileFilter
            || DeleteExcludedFilesDefault != _config.DeleteExcludedFilesDefault
            || DeleteEmptyFoldersDefault != _config.DeleteEmptyFoldersDefault;

        public UserFlattenConfigViewModel(UserFlattenConfig config)
        {
            _config = config;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            DefaultRegexFileFilter = _config.DefaultRegexFileFilter;
            DeleteExcludedFilesDefault = _config.DeleteExcludedFilesDefault;
            DeleteEmptyFoldersDefault = _config.DeleteEmptyFoldersDefault;
        }

        public override UserFlattenConfig ApplyChanges()
        {
            _config = new(DefaultRegexFileFilter, DeleteExcludedFilesDefault, DeleteEmptyFoldersDefault);
            OnPropertyChanged(nameof(DefaultRegexFileFilter));
            OnPropertyChanged(nameof(DeleteExcludedFilesDefault));
            OnPropertyChanged(nameof(DeleteEmptyFoldersDefault));
            OnPropertyChanged(nameof(IsDirty));
            return _config;
        }

        public override UserFlattenConfig UndoChanges()
        {
            ApplyConfig();
            return _config;
        }
    }
}
