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
        private string _matchPattern = ".+";
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
        private string _renamePattern = "<Match>";
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

            InputMatchNodes.Clear();
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
                                subNodes.Add(new(RenameMatchNodeType.Tag, i, null, j, groupValues[j].Value, groupName, groupKeys[groupName]));
                            }
                        }

                        InputMatchNodes.Add(new RenameMatchNodeViewModel(RenameMatchNodeType.Match, i, subNodes, isExpanded: i == 0));
                    }
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<RenameMatchNodeViewModel> _inputMatchNodes = new();

        public HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> InputMatchNodeColumns => new(InputMatchNodes)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<RenameMatchNodeViewModel>(
                    new TemplateColumn<RenameMatchNodeViewModel>(Resources.Resources.Name, "MatchNameCell"),
                    x => x.SubNodes, x => x.HasSubNodes, x=> x.IsExpanded),
                new TextColumn<RenameMatchNodeViewModel, string>(Resources.Resources.Value, x => x.Value),
            }
        };

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
                var renamedFileName = RegexRenameMatches(file.Matches, RenamePattern, file.FileNameWithoutExtension, out var renameError);
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

        protected string RegexRenameMatches(IEnumerable<IRenameMatch>? matches, string? renamePattern, string? fallbackValue, out string? error)
        {
            string output = fallbackValue ?? string.Empty;
            error = null;
            if (!string.IsNullOrWhiteSpace(renamePattern))
            {
                matches ??= Enumerable.Empty<IRenameMatch>();
                RenameResult renameResult = _regexProvider.Rename(matches, renamePattern);
                if (renameResult.Errors?.Any() == true)
                {
                    error = string.Join('\n', renameResult.Errors);
                }
                else
                {
                    output = renameResult.Value;
                }
            }

            return output;
        }
    }
}
