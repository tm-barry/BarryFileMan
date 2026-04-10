using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace BarryFileMan.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task<IReadOnlyList<IStorageFile>?> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel != null) 
            {
                return await topLevel.StorageProvider.OpenFilePickerAsync(options);
            }

            return null;
        }

        public async Task<IReadOnlyList<IStorageFolder>?> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel != null)
            {
                return await topLevel.StorageProvider.OpenFolderPickerAsync(options);
            }

            return null;
        }

        public async Task ClipboardSetTextAsync(string? text)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel?.Clipboard != null)
            {
                await topLevel.Clipboard.SetTextAsync(text);
            }
        }
    }
}