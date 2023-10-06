using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BarryFileMan.Managers;
using BarryFileMan.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BarryFileMan.ViewModels
{
    public partial class RenameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<RenameFileViewModel> _files = new();

        [ObservableProperty]
        private RenameFileViewModel? _selectedFile;

        public bool HasFiles => Files.Any();

        public ReadOnlyCollection<RenameLoadOptionViewModel> LoadOptions => new List<RenameLoadOptionViewModel>()
        {
            new RenameLoadOptionViewModel(RenameLoadOption.Files, "Files", "FileMultiple", LoadFilesCommand),
            new RenameLoadOptionViewModel(RenameLoadOption.Folders, "Folders", "FolderMultiple", LoadFoldersCommand),
        }.AsReadOnly();

        [ObservableProperty]
        private RenameLoadOptionViewModel _selectedLoadOption;

        public RenameViewModel()
        {
            Files.CollectionChanged += Files_CollectionChanged;

            // Load default option from config
            SelectedLoadOption = LoadOptions.FirstOrDefault((option) => option.Type == AppManager.UserConfig.Config.DefaultRenameLoadOption) ?? LoadOptions.First();
            AppManager.UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);
        }

        ~ RenameViewModel()
        {
            Files.CollectionChanged -= Files_CollectionChanged;
        }

        private void Files_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasFiles));
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
        private static async Task CopyRenameFile(RenameFileViewModel renameFile)
        {
            if (AppManager.MainWindow != null)
            {
                await AppManager.MainWindow.ClipboardSetTextAsync(renameFile.File.Name);
            }
        }

        [RelayCommand]
        private void RemoveRenameFile(RenameFileViewModel renameFile)
        {
            Files.Remove(renameFile);
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
    }
}
