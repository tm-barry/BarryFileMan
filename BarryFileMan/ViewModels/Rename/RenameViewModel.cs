using Avalonia.Platform.Storage;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Helpers;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using BarryFileMan.Rename.Enums;
using BarryFileMan.ViewModels.Rename.Providers;
using BarryFileMan.Views.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename
{
    public partial class RenameViewModel : ObservableObject
    {
        private Dictionary<RenameProviderTypes, BaseRenameProviderViewModel> _renameProviderCache = new();
        private bool _addingFiles = false;

        [ObservableProperty]
        public bool _isBusy;

        public static ReadOnlyCollection<RenameProviderTypeItemViewModel> AllProviderTypes => new List<RenameProviderTypeItemViewModel>()
        {
            new(RenameProviderTypes.Regex, Resources.Resources.Regex, "regex", 24, 24),
            new(RenameProviderTypes.TMDB_Movie, Resources.Resources.Movie, "/Assets/tmdb-alt-short-logo.64x8.png", 8, 64, true),
            new(RenameProviderTypes.TMDB_TV, Resources.Resources.TV, "/Assets/tmdb-alt-short-logo.64x8.png", 8, 64, true)
        }.AsReadOnly();

        public static ReadOnlyCollection<RenameProviderTypeItemViewModel> ProviderTypes => AllProviderTypes.Where(pt =>
            pt.Type != RenameProviderTypes.TMDB_Movie && pt.Type != RenameProviderTypes.TMDB_TV
            || !string.IsNullOrWhiteSpace(AppManager.UserConfig.Config.Tmdb.ApiKey)).ToList().AsReadOnly();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedProviderType))]
        private int? _selectedProviderTypeIndex;
        partial void OnSelectedProviderTypeIndexChanged(int? value)
        {
            BaseRenameProviderViewModel? renameProvider = null;

            if (SelectedProviderType?.Type != null
                && _renameProviderCache.ContainsKey(SelectedProviderType.Type))
            {
                renameProvider = _renameProviderCache[SelectedProviderType.Type];
            }
            else
            {
                switch (SelectedProviderType?.Type)
                {
                    case RenameProviderTypes.Regex:
                        renameProvider = new RegexRenameProviderViewModel(this);
                        break;
                    case RenameProviderTypes.TMDB_Movie:
                        renameProvider = new TMDBMovieRenameProviderViewModel(this);
                        break;
                    case RenameProviderTypes.TMDB_TV:
                        renameProvider = new TMDBTvRenameProviderViewModel(this);
                        break;
                    default:
                        RenameProvider = null;
                        break;
                }

                if(renameProvider != null && SelectedProviderType?.Type != null)
                {
                    _renameProviderCache.Add(SelectedProviderType.Type, renameProvider);
                }
            }

            RenameProvider = renameProvider;
            ProviderSettingsExpanded = true;
        }

        public RenameProviderTypeItemViewModel? SelectedProviderType => ProviderTypes.ElementAtOrDefault(SelectedProviderTypeIndex ?? -1);

        [ObservableProperty]
        private bool _providerSettingsExpanded;

        [ObservableProperty]
        private BaseRenameProviderViewModel? _renameProvider;

        [ObservableProperty]
        private ObservableCollection<RenameFileViewModel> _files = new();

        [ObservableProperty]
        private RenameFileViewModel? _selectedFile;

        public bool HasFiles => Files.Any();

        public ReadOnlyCollection<RenameLoadOptionViewModel> LoadOptions => new List<RenameLoadOptionViewModel>()
        {
            new(RenameLoadOption.Files, Resources.Resources.Files, "FileMultiple", LoadFilesCommand),
            new(RenameLoadOption.Folders, Resources.Resources.Folders, "FolderMultiple", LoadFoldersCommand),
        }.AsReadOnly();

        [ObservableProperty]
        private RenameLoadOptionViewModel? _selectedLoadOption;

        public RenameViewModel()
        {
            Files.CollectionChanged += Files_CollectionChanged;

            // Load default option from config
            AppManager.UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);

            SelectedProviderTypeIndex = 0;
        }

        ~ RenameViewModel()
        {
            Files.CollectionChanged -= Files_CollectionChanged;
        }

        private void Files_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
                foreach (RenameFileViewModel item in e.NewItems)
                    item.PropertyChanged += File_PropertyChanged;

            if (e?.OldItems != null)
                foreach (RenameFileViewModel item in e.OldItems)
                    item.PropertyChanged -= File_PropertyChanged;

            if (_addingFiles) return;
            
            ApplyFileRenamesCommand.NotifyCanExecuteChanged();
            SaveFileRenamesCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(HasFiles));
        }

        private void File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_addingFiles) return;
            
            if (e.PropertyName == nameof(RenameFileViewModel.HasRenamedFileName))
                SaveFileRenamesCommand.NotifyCanExecuteChanged();
        }

        private void OnUserConfigChanged((UserConfig config, string? key) value)
        {
            SelectedLoadOption = LoadOptions.FirstOrDefault((option) => option.Type == value.config.Rename.DefaultLoadOption) ?? LoadOptions.First();
            OnPropertyChanged(nameof(ProviderTypes));
        }

        [RelayCommand]
        private async Task LoadFolders()
        {
            IsBusy = true;
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Folders);
            var folders = await AppManager.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = Resources.Resources.LoadFolder,
                AllowMultiple = true,
            });
            await AddStorageItems(folders);
            IsBusy = false;
        }

        [RelayCommand]
        private async Task LoadFiles()
        {
            IsBusy = true;
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Files);
            var files = await AppManager.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Resources.Resources.LoadFiles,
                AllowMultiple = true,
            });
            await AddStorageItems(files);
            IsBusy = false;
        }

        [RelayCommand]
        private void ClearFiles()
        {
            SelectedFile = null;
            Files.Clear();
        }

        [RelayCommand]
        private static async Task CopyOriginalFile(RenameFileViewModel renameFile)
        {
            await AppManager.CopyText(Uri.UnescapeDataString(renameFile.File.Path.LocalPath));
        }

        [RelayCommand]
        private static async Task CopyNewFile(RenameFileViewModel renameFile)
        {
            await AppManager.CopyText(renameFile.RenamedFullPath);
        }

        [RelayCommand]
        private void RemoveRenameFile(RenameFileViewModel renameFile)
        {
            Files.Remove(renameFile);
        }

        [RelayCommand(CanExecute = nameof(CanApplyFileRenames))]
        private async Task ApplyFileRenames()
        {
            if (RenameProvider != null)
            {
                IsBusy = true;

                try
                {
                    await RenameProvider.ApplyFileRenames();
                }
                catch (Exception ex) 
                {
                    await AppManager.ExceptionMsgBoxShowWindowDialogAsync(ex);
                }

                IsBusy = false;
            }
        }

        private bool CanApplyFileRenames()
        {
            return (Files?.Any() ?? false);
        }

        [RelayCommand(CanExecute = nameof(CanSaveFileRenames))]
        private async Task SaveFileRenames()
        {
            (MsgBoxResult result, string? input)? result = null;
            if(Files.Any((file) => file.RenameError != null))
                result = await AppManager.MsgBoxShowWindowDialogAsync(
                    Resources.Resources.Error, Resources.Resources.RenameFilesSaveErrorsConfirmation, MsgBoxButtons.YesNo, MsgBoxIcons.Error);

            result ??= await AppManager.MsgBoxShowWindowDialogAsync(
                    Resources.Resources.Rename, Resources.Resources.RenameFilesSaveConfirmation, MsgBoxButtons.YesNo, MsgBoxIcons.Question);

            if(result?.result == MsgBoxResult.Yes)
            {
                var filesToSave = Files.Where((file) => file.RenameError == null).ToList();
                var failedFiles = new List<string>();
                foreach(var file in filesToSave)
                {
                    try
                    {
                        if (file.RenamedFullPath != null)
                        {
                            System.IO.File.Move(file.FullPath, file.RenamedFullPath);
                            Files.Remove(file);
                        }
                        else
                        {
                            failedFiles.Add(file.RelativePathWithoutExtension + file.Extension);
                        }
                    }
                    catch
                    {
                        failedFiles.Add(file.RelativePathWithoutExtension + file.Extension);
                    }
                }

                if(failedFiles.Count > 0)
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        Resources.Resources.Error, 
                        string.Format(Resources.Resources.FilesFailedToRenameMessage, string.Join('\n', failedFiles)), 
                        MsgBoxButtons.Ok, 
                        MsgBoxIcons.Error);
                else
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        Resources.Resources.Success, Resources.Resources.FilesRenamedSuccessMessage, MsgBoxButtons.Ok, MsgBoxIcons.Success);
            }
        }

        private bool CanSaveFileRenames()
        {
            return Files.Any((file) => file.HasRenamedFileName);
        }

        private async Task AddStorageItems(IEnumerable<IStorageItem>? items)
        {
            if (items == null || !items.Any())
                return;

            var files = await StorageItemHelper.GetStorageFiles(items);
            var newItems = await Task.Run(() =>
            {
                var existing = new HashSet<string>(
                    Files.Select(f => f.FullPath),
                    StringComparer.OrdinalIgnoreCase);

                var result = new List<RenameFileViewModel>();
                foreach (var file in files)
                {
                    var fullPath = Uri.UnescapeDataString(file.Path.LocalPath);
                    if (existing.Add(fullPath))
                    {
                        result.Add(new RenameFileViewModel(file));
                    }
                }
                return result;
            });

            // Sort
            var sorted = newItems
                .OrderBy(f => f.File.Name)
                .ToList();

            // Batch UI updates
            _addingFiles = true;
            try
            {
                foreach (var batch in sorted.Chunk(200))
                {
                    foreach (var item in batch)
                        Files.Add(item);
                    
                    await Task.Delay(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _addingFiles = false;
                ApplyFileRenamesCommand.NotifyCanExecuteChanged();
                SaveFileRenamesCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(HasFiles));
                SaveFileRenamesCommand.NotifyCanExecuteChanged();
                SelectedFile = Files.FirstOrDefault();
            }
        }
    }
}
