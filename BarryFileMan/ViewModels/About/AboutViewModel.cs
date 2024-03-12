using BarryFileMan.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace BarryFileMan.ViewModels.About
{
    public partial class AboutViewModel : ObservableObject
    {
        public static string? AppName => Resources.Resources.AppName;
        public static string? AppVersion => AppManager.AppVersion;
        public static string AppDescription => Resources.Resources.AppDescription;
        public static Uri AppUri => new(Resources.Resources.AppUrl);
    }
}
