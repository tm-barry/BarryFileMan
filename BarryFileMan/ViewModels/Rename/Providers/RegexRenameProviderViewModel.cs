using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Managers;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Models.Regex;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RegexRenameProviderViewModel : BaseRenameProviderViewModel
    {
        private readonly RegexRenameProvider _provider = new();

        [ObservableProperty]
        [HasErrorProperty(nameof(MatchPatternError))]
        private string _matchPattern = "(?:\\\\|/)(?<title>[^(?:\\\\|/)]+)(?:s|S)(?<season>\\d+)(?:e|E)(?<episode>\\d+)";
        partial void OnMatchPatternChanged(string value)
        {
            TestMatches = FindMatches(TestString, value, out var error);
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
            new(RegexRenameMatchTypes.Filename, "Filename", false),
            new(RegexRenameMatchTypes.FilenameDirectory, "Filename & Directory", false),
            new(RegexRenameMatchTypes.FullPath, "Full Path", false)
        }.AsReadOnly();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedMatchType))]
        private int? _selectedMatchTypeIndex;
        partial void OnSelectedMatchTypeIndexChanged(int? value)
        {
            if (ViewModel.SelectedFile != null)
            {
                TestString = GetFileMatchInput(ViewModel.SelectedFile);
            }
        }

        public RegexRenameMatchTypes SelectedMatchType => MatchTypes.ElementAtOrDefault(SelectedMatchTypeIndex ?? -1)?.Item ?? RegexRenameMatchTypes.Filename;

        [ObservableProperty]
        [HasErrorProperty(nameof(RenamePatternError))]
        private string _renamePattern = "<title{-1}.replace(\'.\',\' \')>- S<season{-1}.pad(left,\'0\',2)>E<episode{-1}.pad(left,\'0\',2)>";
        partial void OnRenamePatternChanged(string value)
        {
            RenamedTestString = RenameMatches(TestMatches, value, TestString, out var error);
            RenamePatternError = error;
        }

        [ObservableProperty]
        private string? _renamePatternError;
        partial void OnRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(RenamePattern, nameof(RenamePattern));
        }

        [ObservableProperty]
        private string _testString = string.Empty;
        partial void OnTestStringChanged(string value)
        {
            TestMatches = FindMatches(value, MatchPattern, out var error);
            MatchPatternError = error;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTestMatches))]
        private IEnumerable<IRenameMatch>? _testMatches;
        partial void OnTestMatchesChanged(IEnumerable<IRenameMatch>? value)
        {
            RenamedTestString = RenameMatches(value, RenamePattern, TestString, out var error);
            RenamePatternError = error;

            TestMatchNodes.Clear();
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

                        TestMatchNodes.Add(new RenameMatchNodeViewModel(RenameMatchNodeType.Match, i, subNodes, isExpanded: i == 0));
                    }
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<RenameMatchNodeViewModel> _testMatchNodes = new();

        public HierarchicalTreeDataGridSource<RenameMatchNodeViewModel> TestMatchNodeColumns => new(TestMatchNodes)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<RenameMatchNodeViewModel>(
                    new TemplateColumn<RenameMatchNodeViewModel>("Name", "MatchNameCell"),
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
            TestString = "\\ParentFolder\\Show.Name.S01E01.1080p.x265-TEST";
            SelectedMatchTypeIndex = 1;
        }

        ~RegexRenameProviderViewModel()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        public override void ApplyFileRenames()
        {
            foreach(var file in ViewModel.Files)
            {
                file.IsDuplicate = false;
                file.Matches = FindMatches(GetFileMatchInput(file), MatchPattern, out _);
                var renamedFileName = RenameMatches(file.Matches, RenamePattern, file.FileNameWithoutExtension, out var renameError);
                file.RenameError = renameError;
                file.RenamedFileName = string.IsNullOrEmpty(renameError) ? renamedFileName : null;
            }

            HandleDuplicateFilenames();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SelectedFile))
            {
                if (ViewModel.SelectedFile != null)
                {
                    TestString = GetFileMatchInput(ViewModel.SelectedFile);
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

        private IEnumerable<IRenameMatch>? FindMatches(string? input, string? regexPattern, out string? error)
        {
            IEnumerable<IRenameMatch>? matches = null;
            error = null;
            if (!string.IsNullOrWhiteSpace(regexPattern))
            {
                try
                {
                    matches = _provider.Match(input ?? string.Empty, new RegexRenameProviderMatchOptions { RegexPattern = regexPattern });
                }
                catch (InvalidRegexException ex)
                {
                    error = ex.Message;
                    matches = null;
                }
            }

            return matches;
        }

        private string RenameMatches(IEnumerable<IRenameMatch>? matches, string? renamePattern, string? fallbackValue, out string? error)
        {
            string renamedString = fallbackValue ?? string.Empty;
            error = null;
            if (!string.IsNullOrWhiteSpace(renamePattern))
            {
                matches ??= Enumerable.Empty<IRenameMatch>();
                RenameResult renameResult = _provider.Rename(matches, renamePattern);
                if (renameResult.Errors?.Any() == true)
                {
                    error = string.Join('\n', renameResult.Errors);
                }
                else
                {
                    renamedString = renameResult.Value;
                }
            }

            return renamedString;
        }
    }
}
