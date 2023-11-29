using BarryFileMan.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.About
{
    public partial class AboutViewModel : ObservableObject
    {
        public static string? AppName => AppManager.AppName;
        public static string? AppVersion => AppManager.AppVersion;
        public static string AppDescription => $"{AppName} is a file manager with a primary focus of renaming and managing media files (tv shows, movies, books, and comics).";
        public static Uri AppUri => new("https://github.com/tm-barry/BarryFileMan");

        [RelayCommand]
        private static void OpenAppUri()
        {
            Process.Start(new ProcessStartInfo() { FileName = AppUri.ToString(), UseShellExecute = true });
        }
    }
}
