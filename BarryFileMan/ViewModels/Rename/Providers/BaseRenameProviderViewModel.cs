using BarryFileMan.ViewModels.Flatten;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public abstract partial class BaseRenameProviderViewModel : ObservableValidator
    {
        public RenameViewModel ViewModel { get; private set; }

        public BaseRenameProviderViewModel(RenameViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public abstract void ApplyFileRenames();

        protected void HandleDuplicateFilenames()
        {
            foreach (var file in ViewModel.Files.Where((f) => f.RenamedFileName != null))
            {
                var dupFiles = ViewModel.Files.Where((df) => df.FullPath != file.FullPath && df.RenamedFullPath == file.RenamedFullPath);
                if (dupFiles.Any())
                {
                    file.IsDuplicate = true;
                }

                foreach (var dupFile in dupFiles)
                {
                    dupFile.IsDuplicate = true;
                    dupFile.RenamedFileName = GetNextAvailableFilename(dupFile.RenamedFileName ?? string.Empty, dupFiles);
                }
            }
        }

        private static string GetNextAvailableFilename(string filename, IEnumerable<RenameFileViewModel> files)
        {
            if (!files.Any((file) => file.RenamedFileName == filename)) return filename;

            string alternateFilename;
            int fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                alternateFilename = string.Format("{0}({1})", filename, fileNameIndex);
            } while (files.Any((file) => file.RenamedFileName == alternateFilename));

            return alternateFilename;
        }
    }
}
