namespace BarryFileMan.Models.Presets
{
    public class RenameRegexPreset
    {
        public string? MatchPattern { get; set; }
        public string? RenamePattern { get; set; }
        public string? Input { get; set; }
        public int? SelectedMatchTypeIndex { get; set; }
    }
}
