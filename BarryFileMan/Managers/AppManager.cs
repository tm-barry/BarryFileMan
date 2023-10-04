using BarryFileMan.Managers.Config;
using BarryFileMan.Models.Config;

namespace BarryFileMan.Managers
{
    public static class AppManager
    {
        public static IConfigManager<UserConfig> UserConfig { get; set; } = new JsonUserConfigManager();
    }
}
