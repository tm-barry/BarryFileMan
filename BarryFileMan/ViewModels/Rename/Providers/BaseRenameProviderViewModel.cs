using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BarryFileMan.Interfaces;
using BarryFileMan.Managers;
using BarryFileMan.Models.Presets;
using System;
using System.IO;
using System.Text.RegularExpressions;
using BarryFileMan.Rename.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public abstract partial class BaseRenameProviderViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SavePresetCommand))]
        [NotifyCanExecuteChangedFor(nameof(RenamePresetCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveNewPresetCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeletePresetCommand))]
        private bool _isBusy;
        protected virtual void OnIsBusyChangedBefore(bool value) { }
        partial void OnIsBusyChanged(bool value)
        {
            OnIsBusyChangedBefore(value);
        }

        [ObservableProperty]
        private IList<IPresetViewModel> _presets = new List<IPresetViewModel>();

        private IPresetViewModel? _selectedPreset;
        protected virtual void OnSelectedPresetChanged(IPresetViewModel? value) { }
        public IPresetViewModel? SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (_selectedPreset != value && value != null)
                {
                    _selectedPreset = value;
                    OnPropertyChanged(nameof(SelectedPreset));
                    SavePresetCommand.NotifyCanExecuteChanged();
                    RenamePresetCommand.NotifyCanExecuteChanged();
                    SaveNewPresetCommand.NotifyCanExecuteChanged();
                    DeletePresetCommand.NotifyCanExecuteChanged();
                    OnSelectedPresetChanged(value);
                }
            }
        }

        public RenameViewModel ViewModel { get; private set; }

        public BaseRenameProviderViewModel(RenameViewModel viewModel)
        {
            ViewModel = viewModel;
            AppManager.PresetsConfig.ConfigObservable.Subscribe(OnPresetsConfigChanged);
        }

        public virtual Task ApplyFileRenames()
        {
            HandleDuplicateFilenames();
            return Task.CompletedTask;
        }

        protected virtual void OnPresetsConfigChanged((Presets presets, string? key) value) { }

        protected string RenameMatches(IRenameProvider provider, IEnumerable<IRenameMatch>? matches, string? renamePattern, out string? error,
            string? defaultTagFallbackValue = null)
        {
            error = null;
            matches ??= Enumerable.Empty<IRenameMatch>();
            RenameResult renameResult = provider.Rename(matches, renamePattern ?? string.Empty, defaultTagFallbackValue);
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

        protected void HandleDuplicateFilenames()
        {
            foreach (var file in ViewModel.Files)
                file.IsDuplicate = false;

            var groups = ViewModel.Files
                .GroupBy(f => f.RenamedFullPath ?? f.FullPath)
                .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var ordered = group
                    .OrderBy(f => f.HasRenamedFileName ? 1 : 0);

                foreach (var file in ordered)
                {
                    var baseName = file.HasRenamedFileName
                        ? file.CleanRenamedFileName!
                        : file.FileNameWithoutExtension!;

                    file.RenamedFileName =
                        RenameHelper.GetNextAvailableFileName(baseName, usedNames);

                    file.IsDuplicate = true;
                }
            }
        }

        private bool CanSavePreset() => !IsBusy && SelectedPreset != null && !SelectedPreset.IsSystem;

        [RelayCommand(CanExecute = nameof(CanSavePreset))]
        protected virtual Task SavePreset() => Task.CompletedTask;

        private bool CanRenamePreset() => !IsBusy && SelectedPreset != null && !SelectedPreset.IsSystem;

        [RelayCommand(CanExecute = nameof(CanRenamePreset))]
        protected virtual Task RenamePreset() => Task.CompletedTask;

        private bool CanSaveNewPreset() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanSaveNewPreset))]
        protected virtual Task SaveNewPreset() => Task.CompletedTask;

        private bool CanDeletePreset() => !IsBusy && SelectedPreset != null && !SelectedPreset.IsSystem;

        [RelayCommand(CanExecute = nameof(CanDeletePreset))]
        protected virtual Task DeletePreset() => Task.CompletedTask;
    }
}
