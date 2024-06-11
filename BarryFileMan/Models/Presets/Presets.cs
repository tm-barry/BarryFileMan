namespace BarryFileMan.Models.Presets
{
    public class Presets
    {
        public RenamePresets Rename { get; set; }

        public Presets() : this(new()) { }
        public Presets(RenamePresets rename)
        {
            Rename = rename;
        }
    }
}
