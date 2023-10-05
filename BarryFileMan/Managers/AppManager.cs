using Avalonia.Controls;
using BarryFileMan.Managers.Config;
using BarryFileMan.Models.Config;
using BarryFileMan.Views;

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
    }
}
