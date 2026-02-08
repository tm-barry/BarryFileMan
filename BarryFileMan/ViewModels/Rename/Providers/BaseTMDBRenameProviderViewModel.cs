using Avalonia.Controls;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Managers;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class BaseTMDBRenameProviderViewModel : BaseRegexRenameProviderViewModel
    {
        [ObservableProperty]
        [HasErrorProperty(nameof(QueryRenamePatternError))]
        private string _queryRenamePattern = string.Empty;
        partial void OnQueryRenamePatternChanged(string value)
        {
            HandleQueryRenamePatternChanged(value, InputMatches);
        }

        [ObservableProperty]
        private string? _queryRenamePatternError;
        partial void OnQueryRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(QueryRenamePattern, nameof(QueryRenamePattern));
        }

        [ObservableProperty]
        private string _queryOutput = string.Empty;
        protected virtual void OnQueryOutputChangedBefore(string value) { }
        partial void OnQueryOutputChanged(string value)
        {
            OnQueryOutputChangedBefore(value);
        }

        [ObservableProperty]
        [HasErrorProperty(nameof(YearRenamePatternError))]
        private string _yearRenamePattern = string.Empty;
        partial void OnYearRenamePatternChanged(string value)
        {
            HandleYearRenamePatternChanged(value, InputMatches);
        }

        [ObservableProperty]
        private string? _yearRenamePatternError;
        partial void OnYearRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(YearRenamePattern, nameof(YearRenamePattern));
        }

        [ObservableProperty]
        private string _yearOutput = string.Empty;
        protected virtual void OnYearOutputChangedBefore(string value) { }
        partial void OnYearOutputChanged(string value)
        {
            OnYearOutputChangedBefore(value);
        }

        [ObservableProperty]
        [HasErrorProperty(nameof(LanguageRenamePatternError))]
        private string _languageRenamePattern = string.Empty;
        partial void OnLanguageRenamePatternChanged(string value)
        {
            HandleLanguageRenamePatternChanged(value, InputMatches);
        }

        [ObservableProperty]
        private string? _languageRenamePatternError;
        partial void OnLanguageRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(LanguageRenamePattern, nameof(LanguageRenamePattern));
        }

        [ObservableProperty]
        private string _languageOutput = string.Empty;
        protected virtual void OnLanguageOutputChangedBefore(string value) { }
        partial void OnLanguageOutputChanged(string value)
        {
            OnLanguageOutputChangedBefore(value);
        }

        [ObservableProperty]
        private int _settingsTabIndex;
        partial void OnSettingsTabIndexChanged(int value)
        {
            MatchesTabIndex = value;
        }

        [ObservableProperty]
        private int _matchesTabIndex;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTmdbMatches))]
        private IEnumerable<BarryFileMan.Rename.Interfaces.IRenameMatch>? _tmdbMatches;
        protected virtual void OnTmdbMatchesChangedBefore(IEnumerable<IRenameMatch>? value) { }
        partial void OnTmdbMatchesChanged(IEnumerable<IRenameMatch>? value)
        {
            OnTmdbMatchesChangedBefore(value);
            PopulateMatchNodes(value, TmdbMatchNodes, new List<string>() { "tmdbId", "tmdbOverview", "tmdbPosterPath" });
        }

        [ObservableProperty]
        private ObservableCollection<RenameMatchNodeViewModel> _tmdbMatchNodes = new();

        public HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> TmdbMatchNodeColumns => CreateMatchNodeColumns(TmdbMatchNodes);

        public bool HasTmdbMatches => TmdbMatches != null && TmdbMatches.Any();

        [ObservableProperty]
        private string _output = string.Empty;

        public BaseTMDBRenameProviderViewModel() : this(new RenameViewModel()) { }
        public BaseTMDBRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel) 
        {
            GetTMDBConfig();
        }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleQueryRenamePatternChanged(QueryRenamePattern, value);
            HandleYearRenamePatternChanged(YearRenamePattern, value);
            HandleLanguageRenamePatternChanged(LanguageRenamePattern, value);

            base.OnInputMatchesChangedBefore(value);
        }

        private async void GetTMDBConfig()
        {
            IsBusy = true;
            await AppManager.GetTMDBConfig();
            IsBusy = false;
        }

        private void HandleQueryRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            QueryOutput = RenameMatches(_regexProvider, inputMatches, renamePattern, out var error);
            QueryRenamePatternError = error;
        }

        private void HandleYearRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            YearOutput = RenameMatches(_regexProvider, inputMatches, renamePattern, out var error);
            YearRenamePatternError = error;
        }

        private void HandleLanguageRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            LanguageOutput = RenameMatches(_regexProvider, inputMatches, renamePattern, out var error);
            LanguageRenamePatternError = error;
        }
    }
}
