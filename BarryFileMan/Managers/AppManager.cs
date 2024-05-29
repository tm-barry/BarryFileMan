using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BarryFileMan.Enums.Help;
using BarryFileMan.Helpers;
using BarryFileMan.Managers.Config;
using BarryFileMan.Models.Config;
using BarryFileMan.Rename.Models.TMDB;
using BarryFileMan.Rename.Repositories;
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

        public static TMDBConfiguration? TMDBConfig { get; private set; }

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

        public static async Task GetTMDBConfig()
        {
            if(TMDBConfig == null && !string.IsNullOrWhiteSpace(UserConfig.Config.Tmdb.ApiKey))
            {
                try
                {
                    TMDBConfig = await new TMDBRepository(UserConfig.Config.Tmdb.ApiKey).GetConfigurationAsync();
                }
                catch { }
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

        public static Task HelpWindowShowAsync(HelpSections section = HelpSections.Help, bool isDialog = false, 
            WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            return HelpWindow.ShowAsync(section, isDialog ? MainWindow : null, windowStartupLocation);
        }
    }
}
