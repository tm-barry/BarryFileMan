using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels
{
    public partial class RenameFileViewModel : ViewModelBase
    {
        public IStorageFile File { get; private set; }

        public string? FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(File.Name);

        public string? Extension => System.IO.Path.GetExtension(File.Name);

        [ObservableProperty]
        private string? _renamedFileName;

        public RenameFileViewModel(IStorageFile file)
        {
            File = file;
            RenamedFileName = file.Name;
        }
    }
}
