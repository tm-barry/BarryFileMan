using BarryFileMan.Models.Presets;

namespace BarryFileMan.Managers.Config
{
    public class JsonPresetsConfigManager : BaseJsonConfigManager<Presets>
    {
        protected override string FileName => "presets.json";
    }
}
