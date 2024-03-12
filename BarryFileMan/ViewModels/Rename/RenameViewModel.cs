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
        public static ReadOnlyCollection<RenameProviderTypeItemViewModel> ProviderTypes => new List<RenameProviderTypeItemViewModel>()
        {
            new(RenameProviderTypes.Regex, Resources.Resources.Regex, "regex")
        }.AsReadOnly();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedProviderType))]
        private int? _selectedProviderTypeIndex;
        partial void OnSelectedProviderTypeIndexChanged(int? value)
        {
            switch (SelectedProviderType?.Type)
            {
                case RenameProviderTypes.Regex:
                    RenameProvider = new RegexRenameProviderViewModel(this);
                    break;
                default:
                    RenameProvider = null;
                    break;
            }

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
        private RenameLoadOptionViewModel _selectedLoadOption;

        public RenameViewModel()
        {
            Files.CollectionChanged += Files_CollectionChanged;

            // Load default option from config
            SelectedLoadOption = LoadOptions.FirstOrDefault((option) => option.Type == AppManager.UserConfig.Config.Rename.DefaultLoadOption) ?? LoadOptions.First();
            AppManager.UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);

            // TODO - make this default configurable through settings when we add more provider types
            SelectedProviderTypeIndex = 0;
        }

        ~ RenameViewModel()
        {
            Files.CollectionChanged -= Files_CollectionChanged;
        }

        private void Files_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplyFileRenamesCommand.NotifyCanExecuteChanged();
            SaveFileRenamesCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(HasFiles));

            if (e?.NewItems != null)
                foreach (RenameFileViewModel item in e.NewItems)
                    item.PropertyChanged += File_PropertyChanged;

            if (e?.OldItems != null)
                foreach (RenameFileViewModel item in e.OldItems)
                    item.PropertyChanged -= File_PropertyChanged;
        }

        private void File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveFileRenamesCommand.NotifyCanExecuteChanged();
        }

        private void OnUserConfigChanged(UserConfig userConfig)
        {
            var option = LoadOptions.FirstOrDefault((option) => option.Type == userConfig.Rename.DefaultLoadOption);
            if (option != null)
            {
                SelectedLoadOption = option;
            }
        }

        [RelayCommand]
        private async Task LoadFolders()
        {
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Folders);
            var folders = await AppManager.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = Resources.Resources.LoadFolder,
                AllowMultiple = true,
            });
            await AddStorageItems(folders);
        }

        [RelayCommand]
        private async Task LoadFiles()
        {
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Files);
            var files = await AppManager.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Resources.Resources.LoadFiles,
                AllowMultiple = true,
            });
            await AddStorageItems(files);
        }

        [RelayCommand]
        private void ClearFiles()
        {
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
        private void ApplyFileRenames()
        {
            RenameProvider?.ApplyFileRenames();
        }

        private bool CanApplyFileRenames()
        {
            return (Files?.Any() ?? false);
        }

        [RelayCommand(CanExecute = nameof(CanSaveFileRenames))]
        private async Task SaveFileRenames()
        {
            MsgBoxResult? result = null;
            if(Files.Any((file) => file.RenameError != null))
                result = await AppManager.MsgBoxShowWindowDialogAsync(
                    Resources.Resources.Error, Resources.Resources.RenameFilesSaveErrorsConfirmation, MsgBoxButtons.YesNo, MsgBoxIcons.Error);

            result ??= await AppManager.MsgBoxShowWindowDialogAsync(
                    Resources.Resources.Rename, Resources.Resources.RenameFilesSaveConfirmation, MsgBoxButtons.YesNo, MsgBoxIcons.Question);

            if(result == MsgBoxResult.Yes)
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
            if(items != null && items.Any())
            {
                var files = await StorageItemHelper.GetStorageFiles(items);
                foreach(var file in files)
                {
                    if(!Files.Any((f) => f.File.Path == file.Path))
                        Files.Add(new RenameFileViewModel(file));
                }
            }

            SelectedFile = Files.FirstOrDefault();
        }
    }
}
