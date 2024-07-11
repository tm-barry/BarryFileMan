using BarryFileMan.Enums.Config;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels.Settings
{
    public partial class UserGeneralConfigViewModel : BaseUserConfigViewModel<UserGeneralConfig>
    {
        private UserGeneralConfig _config;

        [ObservableProperty]
        private ObservableCollection<ItemViewModel<Theme>> _themes = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedTheme))]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private int? _selectedThemeIndex;

        public Theme SelectedTheme => Themes.ElementAtOrDefault(SelectedThemeIndex ?? -1)?.Item ?? Theme.Default;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private bool _sidebarExpandedDefault;

        public string ConfigPath => AppManager.UserConfig.FolderPath;

        public override bool IsDirty => SelectedTheme != _config.Theme || SidebarExpandedDefault != _config.SidebarExpandedDefault;

        public UserGeneralConfigViewModel(UserGeneralConfig config)
        {
            _config = config;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            Themes.Clear();
            Themes.Add(new ItemViewModel<Theme>(Theme.Default, Resources.Resources.SystemDefault, _config.Theme == Theme.Default, "ThemeLightDark"));
            Themes.Add(new ItemViewModel<Theme>(Theme.Dark, Resources.Resources.Dark, _config.Theme == Theme.Dark, "LightbulbNight"));
            Themes.Add(new ItemViewModel<Theme>(Theme.Light, Resources.Resources.Light, _config.Theme == Theme.Light, "LightbulbOnOutline"));

            var selectedTheme = Themes.FirstOrDefault((theme) => theme.Item == _config.Theme);
            SelectedThemeIndex = selectedTheme != null ? Themes.IndexOf(selectedTheme) : -1;

            SidebarExpandedDefault = _config.SidebarExpandedDefault;
        }

        public override UserGeneralConfig UndoChanges()
        {
            ApplyConfig();
            return _config;
        }

        public override UserGeneralConfig ApplyChanges()
        {
            _config = new(SelectedTheme, SidebarExpandedDefault);
            OnPropertyChanged(nameof(SelectedTheme));
            OnPropertyChanged(nameof(IsDirty));
            return _config;
        }
    }
}
