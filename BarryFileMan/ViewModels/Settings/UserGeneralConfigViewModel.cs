using BarryFileMan.Enums.Config;
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

        public Theme SelectedTheme => Themes.FirstOrDefault((theme) => theme.Selected)?.Item ?? Theme.Default;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDirty))]
        private bool _sidebarExpandedDefault;

        public bool IsDirty => SelectedTheme != _config.Theme || SidebarExpandedDefault != _config.SidebarExpandedDefault;

        public UserGeneralConfigViewModel(UserGeneralConfig config)
        {
            _config = config;
            Themes.CollectionChanged += Themes_CollectionChanged;
            ApplyConfig();
        }

        ~UserGeneralConfigViewModel()
        {
            Themes.CollectionChanged -= Themes_CollectionChanged;
        }

        private void Themes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
                foreach (ItemViewModel<Theme> item in e.NewItems)
                    item.PropertyChanged += Theme_PropertyChanged;

            if (e?.OldItems != null)
                foreach (ItemViewModel<Theme> item in e.OldItems)
                    item.PropertyChanged -= Theme_PropertyChanged;

            OnPropertyChanged(nameof(SelectedTheme));
            OnPropertyChanged(nameof(IsDirty));
        }

        private void Theme_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedTheme));
            OnPropertyChanged(nameof(IsDirty));
        }

        private void ApplyConfig()
        {
            Themes.Clear();
            Themes.Add(new ItemViewModel<Theme>(Theme.Default, Resources.Resources.SystemDefault, _config.Theme == Theme.Default));
            Themes.Add(new ItemViewModel<Theme>(Theme.Dark, Resources.Resources.Dark, _config.Theme == Theme.Dark));
            Themes.Add(new ItemViewModel<Theme>(Theme.Light, Resources.Resources.Light, _config.Theme == Theme.Light));

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
