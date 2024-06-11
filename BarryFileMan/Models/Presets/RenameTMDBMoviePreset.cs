namespace BarryFileMan.Models.Presets
{
    public class RenameTMDBMoviePreset : RenameRegexPreset
    {
        public string? QueryRenamePattern { get; set; }
        public string? YearRenamePattern { get; set; }
        public string? LanguageRenamePattern { get; set; }
    }
}
