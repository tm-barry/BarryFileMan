using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserTMDBConfigViewModel : BaseUserConfigViewModel<UserTMDBConfig>
    {
        private UserTMDBConfig _config;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private string? _apiKey;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private bool _includeAdult;

        public override bool IsDirty => ApiKey != _config.ApiKey
            || IncludeAdult != _config.IncludeAdult;

        public UserTMDBConfigViewModel(UserTMDBConfig config)
        {
            _config = config;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            ApiKey = _config.ApiKey;
            IncludeAdult = _config.IncludeAdult;
        }

        public override UserTMDBConfig ApplyChanges()
        {
            _config = new(ApiKey, IncludeAdult);
            OnPropertyChanged(nameof(ApiKey));
            OnPropertyChanged(nameof(IncludeAdult));
            OnPropertyChanged(nameof(IsDirty));
            return _config;
        }

        public override UserTMDBConfig UndoChanges()
        {
            ApplyConfig();
            return _config;
        }
    }
}
