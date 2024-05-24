using BarryFileMan.Attributes.Validation;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

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

        [ObservableProperty]
        private string _output = string.Empty;

        public BaseTMDBRenameProviderViewModel() : this(new RenameViewModel()) { }
        public BaseTMDBRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel) { }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleQueryRenamePatternChanged(QueryRenamePattern, value);
            HandleYearRenamePatternChanged(YearRenamePattern, value);
            HandleLanguageRenamePatternChanged(LanguageRenamePattern, value);

            base.OnInputMatchesChangedBefore(value);
        }

        private void HandleQueryRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            QueryOutput = RegexRenameMatches(inputMatches, renamePattern, string.Empty, out var error);
            QueryRenamePatternError = error;
        }

        private void HandleYearRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            YearOutput = RegexRenameMatches(inputMatches, renamePattern, string.Empty, out var error);
            YearRenamePatternError = error;
        }

        private void HandleLanguageRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            LanguageOutput = RegexRenameMatches(inputMatches, renamePattern, string.Empty, out var error);
            LanguageRenamePatternError = error;
        }
    }
}
