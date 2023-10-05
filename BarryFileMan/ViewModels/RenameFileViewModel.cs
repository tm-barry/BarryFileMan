using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels
{
    public partial class RenameFileViewModel : ViewModelBase
    {
        public IStorageItem File { get; private set; }

        [ObservableProperty]
        private string? _renamedFileName;

        public RenameFileViewModel(IStorageItem file)
        {
            File = file;
            RenamedFileName = file.Name;
        }
    }
}
