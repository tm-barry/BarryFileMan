using Avalonia.Platform.Storage;
using BarryFileMan.Enums.Rename;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using BarryFileMan.Rename.Enums;
using BarryFileMan.ViewModels.Rename.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
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
            new(RenameProviderTypes.Regex, "Regex", "regex")
        }.AsReadOnly();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedProviderType))]
        private int? _selectedProviderTypeIndex;

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
            new(RenameLoadOption.Files, "Files", "FileMultiple", LoadFilesCommand),
            new(RenameLoadOption.Folders, "Folders", "FolderMultiple", LoadFoldersCommand),
        }.AsReadOnly();

        [ObservableProperty]
        private RenameLoadOptionViewModel _selectedLoadOption;

        public RenameViewModel()
        {
            Files.CollectionChanged += Files_CollectionChanged;

            // Load default option from config
            SelectedLoadOption = LoadOptions.FirstOrDefault((option) => option.Type == AppManager.UserConfig.Config.DefaultRenameLoadOption) ?? LoadOptions.First();
            AppManager.UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);

            // TODO - make this default configurable through settings when we add more provider types
            SelectedProviderTypeIndex = 0;
        }

        ~ RenameViewModel()
        {
            Files.CollectionChanged -= Files_CollectionChanged;
        }

        partial void OnSelectedProviderTypeIndexChanged(int? value)
        {
            if(RenameProvider != null)
            {
                RenameProvider.PropertyChanged -= RenameProvider_PropertyChanged;
            }

            switch (SelectedProviderType?.Type)
            {
                case RenameProviderTypes.Regex:
                    RenameProvider = new RegexRenameProviderViewModel(this);
                    break;
                default:
                    RenameProvider = null;
                    break;
            }

            if (RenameProvider != null) 
            {
                RenameProvider.PropertyChanged += RenameProvider_PropertyChanged;
            }

            ProviderSettingsExpanded = true;
        }

        private void RenameProvider_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(RenameProvider.CanRenameFiles))
            {
                ApplyFileRenamesCommand.NotifyCanExecuteChanged();
            }
        }

        private void Files_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplyFileRenamesCommand.NotifyCanExecuteChanged();
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
            var option = LoadOptions.FirstOrDefault((option) => option.Type == userConfig.DefaultRenameLoadOption);
            if (option != null)
            {
                SelectedLoadOption = option;
            }
        }

        [RelayCommand]
        private async Task LoadFolders()
        {
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Folders);
            if (AppManager.MainWindow != null)
            {
                var folders = await AppManager.MainWindow.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Load Folder",
                    AllowMultiple = true,
                });

                await AddStorageItems(folders);
            }
        }

        [RelayCommand]
        private async Task LoadFiles()
        {
            SelectedLoadOption = LoadOptions.First((item) => item.Type == RenameLoadOption.Files);
            if (AppManager.MainWindow != null)
            {
                var files = await AppManager.MainWindow.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Load Files",
                    AllowMultiple = true,
                });


                await AddStorageItems(files);
            }
        }

        [RelayCommand]
        private void ClearFiles()
        {
            Files.Clear();
        }

        [RelayCommand]
        private static async Task CopyOriginalFile(RenameFileViewModel renameFile)
        {
            await CopyText(renameFile.File.Name);
        }

        [RelayCommand]
        private static async Task CopyNewFile(RenameFileViewModel renameFile)
        {
            // TODO - copy new file name
            await CopyText(renameFile.File.Name);
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
            return (Files?.Any() ?? false) && (RenameProvider?.CanRenameFiles ?? false);
        }

        [RelayCommand(CanExecute = nameof(CanSaveFileRenames))]
        private async Task SaveFileRenames()
        {
            ButtonResult? result = null;
            if(Files.Any((file) => file.RenameError != null))
                result = await AppManager.MsgBoxShowWindowDialogAsync(
                    "Error", "Some files have errors and can't be renamed. Do you still want to continue?", ButtonEnum.YesNo, Icon.Error);

            result ??= await AppManager.MsgBoxShowWindowDialogAsync(
                    "Rename", "Files will be renamed. Do you want to continue?", ButtonEnum.YesNo, Icon.Question);

            if(result == ButtonResult.Yes)
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
                        "Error", $"The following files failed to be renamed:\n\n{string.Join('\n', failedFiles)}", ButtonEnum.Ok, Icon.Error);
                else
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        "Success", $"Files successfully renamed!", ButtonEnum.Ok, Icon.Success);
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
                foreach(var item in items)
                {
                    if (item is IStorageFile file)
                    {
                        AddStorageFile(file);
                    }
                    else if (item is IStorageFolder folder)
                    {
                        await AddStorageFolder(folder);
                    }
                }
            }
        }

        private void AddStorageFile(IStorageFile file)
        {
            Files.Add(new RenameFileViewModel(file));
        }

        private async Task AddStorageFolder(IStorageFolder folder)
        {
            var storageItems = await folder.GetItemsAsync().ToListAsync();
            await AddStorageItems(storageItems);
        }

        private static async Task CopyText(string? text)
        {
            if (AppManager.MainWindow != null)
            {
                await AppManager.MainWindow.ClipboardSetTextAsync(text);
            }
        }
    }
}
