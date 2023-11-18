using Avalonia.Platform.Storage;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename
{
    public partial class RenameFileViewModel : ObservableObject
    {
        public IStorageFile File { get; private set; }

        public string? FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(File.Name);

        public string RelativePath => System.IO.Path.Combine(
            System.IO.Path.DirectorySeparatorChar.ToString(),
            System.IO.Directory.GetParent(File.Path.AbsolutePath)?.Name ?? string.Empty,
            FileNameWithoutExtension ?? string.Empty);

        public string? FullPath => System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(File.Path.AbsolutePath) ?? string.Empty,
            FileNameWithoutExtension ?? string.Empty);

        public string? Extension => System.IO.Path.GetExtension(File.Name);

        [ObservableProperty]
        public IEnumerable<IRenameMatch>? _matches;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasRenamedFileName))]
        private string? _renamedFileName;

        public bool HasRenamedFileName => !string.IsNullOrWhiteSpace(RenamedFileName);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string? _renameError;

        public bool HasError => RenameError != null;

        public RenameFileViewModel(IStorageFile file)
        {
            File = file;
        }
    }
}
