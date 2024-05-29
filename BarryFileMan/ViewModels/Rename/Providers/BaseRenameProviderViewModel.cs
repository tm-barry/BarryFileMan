using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public abstract partial class BaseRenameProviderViewModel : ObservableValidator
    {
        [ObservableProperty]
        public bool _isBusy;
        protected virtual void OnIsBusyChangedBefore(bool value) { }
        partial void OnIsBusyChanged(bool value)
        {
            OnIsBusyChangedBefore(value);
        }

        public RenameViewModel ViewModel { get; private set; }

        public BaseRenameProviderViewModel(RenameViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public virtual async Task ApplyFileRenames()
        {
            HandleDuplicateFilenames();
        }

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

        protected void HandleDuplicateFilenames()
        {
            foreach (var file in ViewModel.Files.Where((f) => f.RenamedFileName != null))
            {
                file.IsDuplicate = false;
                var dupFiles = ViewModel.Files.Where((df) => df.FullPath != file.FullPath && df.RenamedFullPath == file.RenamedFullPath);
                if (dupFiles.Any())
                {
                    file.IsDuplicate = true;
                }

                foreach (var dupFile in dupFiles)
                {
                    dupFile.IsDuplicate = true;
                    dupFile.RenamedFileName = GetNextAvailableFilename(dupFile.RenamedFileName ?? string.Empty, dupFiles);
                }
            }
        }

        private static string GetNextAvailableFilename(string filename, IEnumerable<RenameFileViewModel> files)
        {
            if (!files.Any((file) => file.RenamedFileName == filename)) return filename;

            string alternateFilename;
            int fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                alternateFilename = string.Format("{0}({1})", filename, fileNameIndex);
            } while (files.Any((file) => file.RenamedFileName == alternateFilename));

            return alternateFilename;
        }
    }
}
