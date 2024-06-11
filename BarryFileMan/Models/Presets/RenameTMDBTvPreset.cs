namespace BarryFileMan.Models.Presets
{
    public class RenameTMDBTvPreset : RenameTMDBMoviePreset
    {
        public string? SeasonRenamePattern { get; set; }
        public string? EpisodeRenamePattern { get; set; }
    }
}
