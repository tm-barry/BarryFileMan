using Avalonia.Platform.Storage;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Rename
{
    public partial class RenameFileViewModel : ObservableObject
    {
        public IStorageFile File { get; private set; }

        public string FullPath => Uri.UnescapeDataString(File.Path.LocalPath);

        public string DirectoryPath => System.IO.Path.GetDirectoryName(FullPath) ?? string.Empty;

        public string? FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(File.Name);

        public string RelativePathWithoutExtension => System.IO.Path.Combine(
            System.IO.Path.DirectorySeparatorChar.ToString(),
            System.IO.Directory.GetParent(FullPath)?.Name ?? string.Empty,
            FileNameWithoutExtension ?? string.Empty);

        public string? FullPathWithoutExtension => System.IO.Path.Combine(
            DirectoryPath,
            FileNameWithoutExtension ?? string.Empty);

        public string? Extension => System.IO.Path.GetExtension(File.Name);

        [ObservableProperty]
        public IEnumerable<IRenameMatch>? _matches;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasRenamedFileName))]
        private string? _renamedFileName;

        public bool HasRenamedFileName => !string.IsNullOrWhiteSpace(RenamedFileName);

        public string? RenamedFullPath => HasRenamedFileName ? System.IO.Path.Combine(DirectoryPath, RenamedFileName + Extension) : null;

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
