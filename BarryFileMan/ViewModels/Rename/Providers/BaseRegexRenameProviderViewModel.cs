using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Interfaces;
using BarryFileMan.Models.Presets;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using BarryFileMan.Rename.Providers.Regex;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class BaseRegexRenameProviderViewModel : BaseRenameProviderViewModel
    {
        protected readonly RegexRenameMatchProvider _regexProvider = new();

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
        protected virtual void OnInputChangedBefore(string value) { }
        partial void OnInputChanged(string value)
        {
            OnInputChangedBefore(value);
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

        public override Task ApplyFileRenames()
        {
            foreach (var file in ViewModel.Files)
            {
                var matches = RegexFindMatches(GetFileMatchInput(file), MatchPattern, out _);
                var renamedFileName = RenameMatches(_regexProvider, matches, RenamePattern, out var renameError);
                file.RenameError = renameError;
                file.RenamedFileName = string.IsNullOrEmpty(renameError) ? renamedFileName : null;
            }

            return base.ApplyFileRenames();
        }

        protected string GetFileMatchInput(RenameFileViewModel? file)
        {
            string? input = null;
            switch (SelectedMatchType)
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

        protected IEnumerable<IRenameMatch>? RegexFindMatches(string? input, string? regexPattern, out string? error)
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
    }
}
