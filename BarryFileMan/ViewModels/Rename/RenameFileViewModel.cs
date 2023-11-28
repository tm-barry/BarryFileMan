using Avalonia.Platform.Storage;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [NotifyPropertyChangedFor(nameof(InvalidRenamedCharacters))]
        [NotifyPropertyChangedFor(nameof(HasInvalidRenamedCharacters))]
        [NotifyPropertyChangedFor(nameof(InvalidRenamedCharactersStr))]
        [NotifyPropertyChangedFor(nameof(CleanRenamedFileName))]
        [NotifyPropertyChangedFor(nameof(HasRenamedFileName))]
        private string? _renamedFileName;

        public IEnumerable<char> InvalidRenamedCharacters => 
            System.IO.Path.GetInvalidFileNameChars().Where((invalidChar) => RenamedFileName?.Contains(invalidChar) ?? false);

        public bool HasInvalidRenamedCharacters => InvalidRenamedCharacters.Any();

        public string InvalidRenamedCharactersStr => string.Join(',', InvalidRenamedCharacters);

        public string? CleanRenamedFileName
        {
            get
            {
                var cleanFileName = RenamedFileName;
                if (!string.IsNullOrWhiteSpace(cleanFileName))
                {
                    foreach(var invalidChar in InvalidRenamedCharacters)
                    {
                        cleanFileName = cleanFileName.Replace(invalidChar.ToString(), string.Empty);
                    }
                }
                return cleanFileName;
            }
        }

        public bool HasRenamedFileName => !string.IsNullOrWhiteSpace(CleanRenamedFileName);

        public string? RenamedFullPath => HasRenamedFileName ? System.IO.Path.Combine(DirectoryPath, CleanRenamedFileName + Extension) : null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string? _renameError;

        public bool HasError => RenameError != null;

        [ObservableProperty]
        private bool _isDuplicate;

        public RenameFileViewModel(IStorageFile file, bool isDuplicate = false)
        {
            File = file;
            IsDuplicate = isDuplicate;
        }
    }
}
