using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BarryFileMan.Helpers;
using BarryFileMan.Managers.Config;
using BarryFileMan.Models.Config;
using BarryFileMan.Views;
using BarryFileMan.Views.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace BarryFileMan.Managers
{
    public static class AppManager
    {
        private static readonly Assembly? _assembly = Assembly.GetEntryAssembly();
        private static readonly FileVersionInfo? _fileVersionInfo = FileVersionInfo.GetVersionInfo(_assembly?.Location ?? string.Empty);
        public static string? AppName => _fileVersionInfo?.ProductName;
        public static string? AppVersion => _fileVersionInfo?.ProductVersion;

        public static MainWindow? MainWindow { get; private set; }
        public static IConfigManager<UserConfig> UserConfig { get; private set; } = new JsonUserConfigManager();

        static AppManager()
        {
            UserConfig.ConfigObservable.Subscribe(OnUserConfigChanged);
        }

        private static void OnUserConfigChanged(UserConfig userConfig)
        {
            ThemeHelper.ChangeTheme(userConfig.General.Theme);
        }

        public static void Init(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public static async Task CopyText(string? text)
        {
            if (MainWindow != null)
            {
                await MainWindow.ClipboardSetTextAsync(text);
            }
        }

        public static async Task<IReadOnlyList<IStorageFile>?> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            return MainWindow != null ? await MainWindow.OpenFilePickerAsync(options) : null;
        }

        public static async Task<IReadOnlyList<IStorageFolder>?> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        {
            return MainWindow != null ? await MainWindow.OpenFolderPickerAsync(options) : null;
        }

        public static Task<MsgBoxResult> MsgBoxShowWindowDialogAsync(string title, string message, 
            MsgBoxButtons buttons, MsgBoxIcons? icon = null, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            return MessageBox.ShowAsync(MainWindow, title, message, buttons, icon, windowStartupLocation);
        }
    }
}
