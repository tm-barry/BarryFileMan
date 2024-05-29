using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class BaseRegexRenameProviderViewModel : BaseRenameProviderViewModel
    {
        protected readonly RegexRenameProvider _regexProvider = new();

        [ObservableProperty]
        [HasErrorProperty(nameof(MatchPatternError))]
        private string _matchPattern = string.Empty;
        partial void OnMatchPatternChanged(string value)
        {
            InputMatches = RegexFindMatches(Input, value, out var error);
            MatchPatternError = error;
        }

        [ObservableProperty]
        private string? _matchPatternError;
        partial void OnMatchPatternErrorChanged(string? value)
        {
            ValidateProperty(MatchPattern, nameof(MatchPattern));
        }

        public static ReadOnlyCollection<ItemViewModel<RegexRenameMatchTypes>> MatchTypes => new List<ItemViewModel<RegexRenameMatchTypes>>()
        {
            new(RegexRenameMatchTypes.Filename, Resources.Resources.Filename, false),
            new(RegexRenameMatchTypes.FilenameDirectory, Resources.Resources.FilenameAndDirectory, false),
            new(RegexRenameMatchTypes.FullPath, Resources.Resources.FullPath, false)
        }.AsReadOnly();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedMatchType))]
        private int? _selectedMatchTypeIndex = 0;
        partial void OnSelectedMatchTypeIndexChanged(int? value)
        {
            if (ViewModel.SelectedFile != null)
            {
                Input = GetFileMatchInput(ViewModel.SelectedFile);
            }
        }

        public RegexRenameMatchTypes SelectedMatchType => MatchTypes.ElementAtOrDefault(SelectedMatchTypeIndex ?? -1)?.Item ?? RegexRenameMatchTypes.Filename;

        [ObservableProperty]
        [HasErrorProperty(nameof(RenamePatternError))]
        private string _renamePattern = string.Empty;
        protected virtual void OnRenamePatternChangedBefore(string value) { }
        partial void OnRenamePatternChanged(string value)
        {
            OnRenamePatternChangedBefore(value);
        }

        [ObservableProperty]
        private string? _renamePatternError;
        partial void OnRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(RenamePattern, nameof(RenamePattern));
        }

        [ObservableProperty]
        private string _input = string.Empty;
        partial void OnInputChanged(string value)
        {
            InputMatches = RegexFindMatches(value, MatchPattern, out var error) ?? new List<IRenameMatch>();
            MatchPatternError = error;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasInputMatches))]
        private IEnumerable<IRenameMatch>? _inputMatches;
        protected virtual void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value) { }
        partial void OnInputMatchesChanged(IEnumerable<IRenameMatch>? value)
        {
            OnInputMatchesChangedBefore(value);
            PopulateMatchNodes(value, InputMatchNodes);
        }

        [ObservableProperty]
        private ObservableCollection<RenameMatchNodeViewModel> _inputMatchNodes = new();

        public HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> InputMatchNodeColumns => CreateMatchNodeColumns(InputMatchNodes);

        public bool HasInputMatches => InputMatches != null && InputMatches.Any();

        public BaseRegexRenameProviderViewModel() : this(new RenameViewModel()) { }

        public BaseRegexRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        ~BaseRegexRenameProviderViewModel()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        public override void ApplyFileRenames()
        {
            foreach (var file in ViewModel.Files)
            {
                file.Matches = RegexFindMatches(GetFileMatchInput(file), MatchPattern, out _);
                var renamedFileName = RegexRenameMatches(file.Matches, RenamePattern, out var renameError);
                file.RenameError = renameError;
                file.RenamedFileName = string.IsNullOrEmpty(renameError) ? renamedFileName : null;
            }

            base.ApplyFileRenames();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SelectedFile))
            {
                if (ViewModel.SelectedFile != null)
                {
                    Input = GetFileMatchInput(ViewModel.SelectedFile);
                }
            }
        }

        private string GetFileMatchInput(RenameFileViewModel? file)
        {
            string? input = null;
            switch(SelectedMatchType)
            {
                case RegexRenameMatchTypes.Filename:
                    input = file?.FileNameWithoutExtension;
                    break;
                case RegexRenameMatchTypes.FilenameDirectory:
                    input = file?.RelativePathWithoutExtension;
                    break;
                case RegexRenameMatchTypes.FullPath:
                    input = file?.FullPathWithoutExtension;
                    break;
            }

            return input ?? string.Empty;
        }

        private IEnumerable<IRenameMatch>? RegexFindMatches(string? input, string? regexPattern, out string? error)
        {
            IEnumerable<IRenameMatch>? matches = null;
            error = null;
            if (!string.IsNullOrWhiteSpace(regexPattern))
            {
                try
                {
                    matches = _regexProvider.Match(new(regexPattern, input ?? string.Empty));
                }
                catch (InvalidRegexException ex)
                {
                    error = ex.Message;
                    matches = null;
                }
            }

            return matches;
        }

        protected string RegexRenameMatches(IEnumerable<IRenameMatch>? matches, string? renamePattern, out string? error, 
            string? defaultTagFallbackValue = null)
        {
            error = null;
            matches ??= Enumerable.Empty<IRenameMatch>();
            RenameResult renameResult = _regexProvider.Rename(matches, renamePattern ?? string.Empty, defaultTagFallbackValue);
            if (renameResult.Errors?.Any() == true)
            {
                error = string.Join('\n', renameResult.Errors);
            }

            return renameResult.Value;
        }

        protected void PopulateMatchNodes(IEnumerable<IRenameMatch>? value, ObservableCollection<RenameMatchNodeViewModel> matchNodes, 
            IEnumerable<string>? excludedGroups = null)
        {
            matchNodes.Clear();
            if (value != null && value.Any())
            {
                Dictionary<string, int> groupKeys = new();
                int currentGroupKey = 0;
                for (int i = 0; i < value.Count(); i++)
                {
                    var renameMatch = value.ElementAtOrDefault(i);
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
                                if (excludedGroups == null || !excludedGroups.Contains(groupName))
                                {
                                    subNodes.Add(new(RenameMatchNodeType.Tag, i, null, j, groupValues[j].Value, groupName, groupKeys[groupName]));
                                }
                            }
                        }

                        matchNodes.Add(new RenameMatchNodeViewModel(RenameMatchNodeType.Match, i, subNodes, isExpanded: i == 0));
                    }
                }
            }
        }

        protected HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> CreateMatchNodeColumns(ObservableCollection<RenameMatchNodeViewModel> matchNodes)
        {
            return new(matchNodes)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<RenameMatchNodeViewModel>(
                        new TemplateColumn<RenameMatchNodeViewModel>(Resources.Resources.Name, "MatchNameCell"),
                        x => x.SubNodes, x => x.HasSubNodes, x=> x.IsExpanded),
                    new TextColumn<RenameMatchNodeViewModel, string>(Resources.Resources.Value, x => x.Value),
                }
            };
        }
    }
}
