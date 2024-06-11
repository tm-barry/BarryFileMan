using BarryFileMan.Models.Config;

namespace BarryFileMan.Managers.Config
{
    public class JsonUserConfigManager : BaseJsonConfigManager<UserConfig>
    {
        protected override string FileName => "settings.json";
    }
}
