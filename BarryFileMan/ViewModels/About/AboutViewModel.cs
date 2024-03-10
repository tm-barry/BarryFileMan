using BarryFileMan.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BarryFileMan.ViewModels.About
{
    public partial class AboutViewModel : ObservableObject
    {
        public static string? AppName => AppManager.AppName;
        public static string? AppVersion => AppManager.AppVersion;
        public static string AppDescription => $"{AppName} is a file manager with a primary focus of renaming and managing media files and folders.";
        public static Uri AppUri => new("https://github.com/tm-barry/BarryFileMan");
    }
}
