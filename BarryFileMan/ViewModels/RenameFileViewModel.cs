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
        [NotifyPropertyChangedFor(nameof(HasRenamedFileName))]
        private string? _renamedFileName;

        public bool HasRenamedFileName => !string.IsNullOrWhiteSpace(RenamedFileName);

        public RenameFileViewModel(IStorageFile file)
        {
            File = file;
        }
    }
}
