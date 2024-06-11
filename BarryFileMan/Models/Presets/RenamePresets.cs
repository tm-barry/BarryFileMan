using System.Collections.Generic;

namespace BarryFileMan.Models.Presets
{
    public class RenamePresets
    {
        public Dictionary<string, RenameRegexPreset> RegexPresets { get; set; }
        public Dictionary<string, RenameTMDBMoviePreset> TmdbMoviePresets { get; set; }
        public Dictionary<string, RenameTMDBTvPreset> TmdbTvPresets { get; set; }

        public RenamePresets() : this(new(), new(), new()) { }
        public RenamePresets(Dictionary<string, RenameRegexPreset> regexPresets, 
            Dictionary<string, RenameTMDBMoviePreset> tmdbMoviePresets, Dictionary<string, RenameTMDBTvPreset> tmdbTvPresets)
        {
            RegexPresets = regexPresets;
            TmdbMoviePresets = tmdbMoviePresets;
            TmdbTvPresets = tmdbTvPresets;
        }
    }
}
