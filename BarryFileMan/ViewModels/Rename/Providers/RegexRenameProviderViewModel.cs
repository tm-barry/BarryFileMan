using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
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
        [HasErrorProperty(nameof(MatchPatternError))]
        private string _matchPattern = "(?<filename>.+)";

        [ObservableProperty]
        private string? _matchPatternError;

        [ObservableProperty]
        [HasErrorProperty(nameof(RenamePatternError))]
        private string _renamePattern = "<filename>";

        [ObservableProperty]
        private string? _renamePatternError;

        [ObservableProperty]
        private string _testString = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTestMatches))]
        private IEnumerable<IRenameMatch>? _testMatches;

        [ObservableProperty]
        private ObservableCollection<RenameMatchNodeViewModel> _testMatchNodes = new();

        public HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> TestMatchNodeColumns => new(TestMatchNodes)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<RenameMatchNodeViewModel>(
                    new TextColumn<RenameMatchNodeViewModel, string>("Name", x => x.DisplayName),
                    x => x.SubNodes, x => x.HasSubNodes, x=> x.IsExpanded),
                new TextColumn<RenameMatchNodeViewModel, string>("Value", x => x.Value),
            }
        };

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

        partial void OnMatchPatternErrorChanged(string? value)
        {
            ValidateProperty(MatchPattern, nameof(MatchPattern));
        }

        partial void OnRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(RenamePattern, nameof(RenamePattern));
        }

        partial void OnTestMatchesChanged(IEnumerable<IRenameMatch>? oldValue, IEnumerable<IRenameMatch>? newValue)
        {
            TestMatchNodes.Clear();
            if(newValue != null && newValue.Any())
            {
                Dictionary<string, int> groupKeys = new();
                int currentGroupKey = 0;
                for (int i = 0; i < newValue.Count(); i++)
                {
                    var renameMatch = newValue.ElementAtOrDefault(i);
                    if (renameMatch != null)
                    {
                        ObservableCollection<RenameMatchNodeViewModel> subNodes = new();
                        foreach (var groupName in renameMatch.Groups.Keys)
                        {
                            if (!groupKeys.ContainsKey(groupName))
                            {
                                groupKeys.Add(groupName, currentGroupKey);
                                currentGroupKey++;
                            }

                            var groupValues = renameMatch.Groups[groupName];
                            for (int j = 0; j < groupValues.Count; j++)
                            {
                                subNodes.Add(new(RenameMatchNodeType.Tag, i, null, j, groupValues[j].Value, groupName, groupKeys[groupName]));
                            }
                        }

                        TestMatchNodes.Add(new RenameMatchNodeViewModel(RenameMatchNodeType.Match, i, subNodes, isExpanded: i == 0));
                    }
                }
            }
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

            RenamePatternError = null;
            matches ??= Enumerable.Empty<IRenameMatch>();
            RenameResult renameResult = _provider.Rename(matches, input);
            if(renameResult.Errors?.Any() == true)
            {
                RenamedTestString = TestString;
                RenamePatternError = string.Join('\n', renameResult.Errors);
            }
            else
            {
                RenamedTestString = renameResult.Value;
            }
        }
    }
}
