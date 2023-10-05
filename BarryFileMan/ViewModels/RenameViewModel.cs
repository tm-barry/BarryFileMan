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

                if (folders != null && folders.Count > 0)
                {
                    foreach (var folder in folders)
                    {
                        var folderItems = await folder.GetItemsAsync().ToListAsync();
                        var files = folderItems.Where((item) => item is IStorageFile).Cast<IStorageFile>();

                        AddFiles(files);
                    }
                }
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

                AddFiles(files);
            }
        }

        private void AddFiles(IEnumerable<IStorageFile>? files)
        {
            if (files != null && files.Count() > 0)
            {
                foreach (var file in files)
                {
                    Files.Add(new RenameFileViewModel(file));
                }
            }
        }
    }
}
