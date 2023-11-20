using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;

namespace BarryFileMan.ViewModels.Flatten
{
    public partial class FlattenFileViewModel : ObservableObject
    {
        public string BasePath { get; private set; }
        public string FullPath { get; private set; }

        public string RelativePath => FullPath.Replace(BasePath, string.Empty);
        public string FileName => Path.GetFileName(FullPath);

        [ObservableProperty]
        private string? _newFileName;

        [ObservableProperty]
        private bool _isDuplicate;

        [ObservableProperty]
        private bool _exclude;

        public FlattenFileViewModel(string basePath, string fullPath, string? newFileName = null, bool isDuplicate = false, bool exclude = false)
        {
            BasePath = basePath;
            FullPath = fullPath;
            NewFileName = newFileName;
            IsDuplicate = isDuplicate;
            Exclude = exclude;
        }

        [RelayCommand]
        private void ToggleExclude()
        {
            Exclude = !Exclude;
        }
    }
}
