using Avalonia.Platform.Storage;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename
{
    public partial class RenameFileViewModel : ViewModelBase
    {
        public IStorageFile File { get; private set; }

        public string? FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(File.Name);

        public string? Extension => System.IO.Path.GetExtension(File.Name);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMatches))]
        public IEnumerable<IRenameMatch>? _matches;

        public bool HasMatches => Matches?.Count() < 0;

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
