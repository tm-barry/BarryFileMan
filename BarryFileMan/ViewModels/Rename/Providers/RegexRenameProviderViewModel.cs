using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RegexRenameProviderViewModel : BaseRenameProviderViewModel
    {
        private readonly RegexRenameProvider _provider = new();

        [ObservableProperty]
        private string _matchPattern = "(?<filename>.+)";

        [ObservableProperty]
        private string? _matchPatternError;

        [ObservableProperty]
        private string _renamePattern = "<filename>";

        [ObservableProperty]
        private IEnumerable<string>? _renamePatternErrors;

        [ObservableProperty]
        private string _testString = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TestMatchNodes))]
        [NotifyPropertyChangedFor(nameof(HasTestMatches))]
        private IEnumerable<IRenameMatch>? _testMatches;

        public IEnumerable<RenameMatchNodeViewModel>? TestMatchNodes
            => TestMatches?.Select((match, matchIndex) => 
            {
                Dictionary<string, int> groupKeys = new();
                int currentGroupKey = 0;
                ObservableCollection<RenameMatchNodeViewModel> subNodes = new();
                foreach(var groupName in match.Groups.Keys)
                {
                    if(!groupKeys.ContainsKey(groupName))
                    {
                        groupKeys.Add(groupName, currentGroupKey);
                        currentGroupKey++;
                    }

                    var groupValues = match.Groups[groupName];
                    for (int i = 0; i < groupValues.Count; i++)
                    {
                        subNodes.Add(new(matchIndex, null, i, groupValues[i].Value, groupName, groupKeys[groupName]));
                    }
                }
                return new RenameMatchNodeViewModel(matchIndex, subNodes);
            });

        public bool HasTestMatches => TestMatches != null && TestMatches.Any();

        [ObservableProperty]
        private string _renamedTestString = string.Empty;

        public RegexRenameProviderViewModel() : this(new RenameViewModel()) { }

        public RegexRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            TestString = "test-filename";
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
            if(string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(regexPattern))
            {
                TestMatches = null;
                return;
            }

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
            if(string.IsNullOrWhiteSpace(input))
            {
                RenamedTestString = TestString;
                return;
            }

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
