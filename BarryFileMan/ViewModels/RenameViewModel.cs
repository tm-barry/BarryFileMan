using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BarryFileMan.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels
{
    public partial class RenameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<RenameFileViewModel> _files = new();

        [ObservableProperty]
        private RenameFileViewModel? _selectedFile;

        [RelayCommand]
        private async Task LoadFolder()
        {
            if (AppManager.MainWindow != null)
            {
                var folders = await AppManager.MainWindow.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
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
            if (AppManager.MainWindow != null)
            {
                var files = await AppManager.MainWindow.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
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
        private async Task CopyRenameFile(RenameFileViewModel renameFile)
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
