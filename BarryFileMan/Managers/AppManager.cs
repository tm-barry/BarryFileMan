using Avalonia.Controls;
using BarryFileMan.Managers.Config;
using BarryFileMan.Models.Config;
using BarryFileMan.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace BarryFileMan.Managers
{
    public static class AppManager
    {
        public static MainWindow? MainWindow { get; private set; }
        public static IConfigManager<UserConfig> UserConfig { get; private set; } = new JsonUserConfigManager();

        public static void Init(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public static Task<ButtonResult> MsgBoxShowWindowDialogAsync(string title, string message, 
            ButtonEnum button = ButtonEnum.Ok, Icon icon = Icon.None, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(title, message, button, icon, windowStartupLocation);
            return msgBox.ShowWindowDialogAsync(MainWindow);
        }
    }
}
