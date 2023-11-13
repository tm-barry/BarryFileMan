using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RegexRenameProviderViewModel : BaseRenameProviderViewModel
    {
        private readonly RegexRenameProvider _provider = new();

        [ObservableProperty]
        private string _matchPattern = string.Empty;

        [ObservableProperty]
        private string? _matchPatternError;

        [ObservableProperty]
        private string _renamePattern = string.Empty;

        [ObservableProperty]
        private IEnumerable<string>? _renamePatternErrors;

        [ObservableProperty]
        private string _testString = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TestMatchesDisplay))]
        private IEnumerable<IRenameMatch>? _testMatches;

        public IEnumerable<RenameMatchViewModel> TestMatchesDisplay 
            => TestMatches?.Select((item, index) => new RenameMatchViewModel(index, item.Groups)) ?? new List<RenameMatchViewModel>();

        [ObservableProperty]
        private string _renamedTestString = string.Empty;

        public RegexRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        ~RegexRenameProviderViewModel()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SelectedFile))
            {
                if (ViewModel.SelectedFile != null)
                {
                    TestString = ViewModel.SelectedFile.FileNameWithoutExtension ?? string.Empty;
                }
            }
        }

        partial void OnMatchPatternChanged(string value)
        {
            FindMatches(TestString, value);
        }

        partial void OnRenamePatternChanged(string value)
        {
            RenameTestString(TestMatches, value);
        }

        partial void OnTestStringChanged(string value)
        {
            FindMatches(value, MatchPattern);
        }

        partial void OnTestMatchesChanged(IEnumerable<IRenameMatch>? value)
        {
            RenameTestString(value, RenamePattern);
        }

        private void FindMatches(string input, string regexPattern)
        {
            MatchPatternError = null;
            try
            {
                TestMatches = _provider.Match(input, new BarryFileMan.Rename.Models.Regex.RegexRenameProviderMatchOptions { RegexPattern = regexPattern });
            }
            catch (InvalidRegexException ex)
            {
                MatchPatternError = ex.Message;
                TestMatches = null;
            }
        }

        private void RenameTestString(IEnumerable<IRenameMatch>? matches, string input)
        {
            RenamePatternErrors = null;
            matches ??= Enumerable.Empty<IRenameMatch>();
            RenameResult renameResult = _provider.Rename(matches, input);
            if(renameResult.Errors?.Any() == true)
            {
                RenamedTestString = TestString;
                RenamePatternErrors = renameResult.Errors;
            }
            else
            {
                RenamedTestString = renameResult.Value;
            }
        }
    }
}
