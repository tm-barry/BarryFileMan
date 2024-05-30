using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Extensions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;
using BarryFileMan.Rename.Repositories;
using System.Text.RegularExpressions;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBTvRenameMatchProvider : BaseRenameMatchProvider<TMDBTvRenameProviderMatchOptions>
    {
        private bool _useCaching;
        private static Dictionary<int, TMDBSearchMovieTV?> _searchTvCache = new();
        private static Dictionary<int, TMDBTvEpisode?> _getTvEpisodeDetailsCache = new();

        public TMDBTvRenameMatchProvider(bool useCaching = false) : base(RenameProviderTypes.TMDB_TV) 
        {
            _useCaching = useCaching;
        }

        public override IEnumerable<IRenameMatch>? Match(TMDBTvRenameProviderMatchOptions? options)
        {
            return Task.Run(() => MatchAsync(options)).GetAwaiter().GetResult();
        }

        public override async Task<IEnumerable<IRenameMatch>?> MatchAsync(TMDBTvRenameProviderMatchOptions? options)
        {
            if (string.IsNullOrWhiteSpace(options?.APIKey))
            {
                throw new TMDBBadResponseException("Invalid API key: You must be granted a valid key.", -2);
            }

            if (string.IsNullOrWhiteSpace(options?.Query))
            {
                throw new TMDBBadResponseException("Invalid Query: Query must not be blank.", -3);
            }

            List<TMDBRenameMatch>? renameMatches = null;
            var tvMatches = await SearchTv(options);
            if (tvMatches?.Results != null && tvMatches.Results.Any())
            {
                renameMatches = new List<TMDBRenameMatch>();
                foreach (var tvMatch in tvMatches.Results)
                {
                    var renameMatch = new TMDBRenameMatch();

                    renameMatch.Groups.Add("tmdbId",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.Id.ToString()) });
                    renameMatch.Groups.Add("tmdbName",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.Name) });
                    renameMatch.Groups.Add("tmdbOriginalName",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.OriginalName) });
                    renameMatch.Groups.Add("tmdbOriginalLanguage",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.OriginalLanguage) });
                    renameMatch.Groups.Add("tmdbReleaseDate",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.ReleaseDate) });
                    if (DateTime.TryParse(tvMatch.ReleaseDate, out var releaseDate))
                    {
                        renameMatch.Groups.Add("tmdbReleaseYear",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(releaseDate.Year.ToString()) });
                    }
                    renameMatch.Groups.Add("tmdbOverview",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.Overview) });
                    renameMatch.Groups.Add("tmdbPosterPath",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvMatch.PosterPath) });

                    try
                    {
                        var tvEpisode = await GetTvEpisodeDetails(options, tvMatch.Id);
                        if (tvEpisode != null)
                        {
                            renameMatch.Groups.Add("tmdbTitle",
                                new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvEpisode.Name) });
                            renameMatch.Groups.Add("tmdbSeason",
                                new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvEpisode.SeasonNumber.ToString()) });
                            renameMatch.Groups.Add("tmdbEpisode",
                                new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(tvEpisode.EpisodeNumber.ToString()) });
                        }
                    }
                    catch { }

                    renameMatches.Add(renameMatch);
                }
            }

            return renameMatches;
        }

        private async Task<TMDBSearchMovieTV?> SearchTv(TMDBTvRenameProviderMatchOptions options)
        {
            TMDBSearchMovieTV? tvMatches = null;
            var tmdbRepo = new TMDBRepository(options.APIKey);
            var query = options.Query.ToLower().Trim();
            var language = options.Language.ToLower().Trim();
            var year = options.Year.ToLower().Trim();
            var hash = _useCaching ? new[] { query, options.IncludeAdult.ToString(), language, year }.GetSequenceHashCode() : 0;

            if (_useCaching && _searchTvCache.ContainsKey(hash))
            {
                tvMatches = _searchTvCache[hash];
            }
            else
            {
                tvMatches = await tmdbRepo.SearchTv(query, null, options.IncludeAdult, language, null, year);

                if (_useCaching)
                {
                    _searchTvCache.Add(hash, tvMatches);
                }
            }

            return tvMatches;
        }

        private async Task<TMDBTvEpisode?> GetTvEpisodeDetails(TMDBTvRenameProviderMatchOptions options, int seriesId)
        {
            TMDBTvEpisode? tvEpisode = null;
            var tmdbRepo = new TMDBRepository(options.APIKey);
            int.TryParse(options.Season, out var season);
            int.TryParse(options.Episode, out var episode);
            var language = options.Language.ToLower().Trim();
            var hash = _useCaching ? new[] { seriesId.ToString(), season.ToString(), episode.ToString(), language }.GetSequenceHashCode() : 0;

            if (_useCaching && _getTvEpisodeDetailsCache.ContainsKey(hash))
            {
                tvEpisode = _getTvEpisodeDetailsCache[hash];
            }
            else
            {
                tvEpisode = await tmdbRepo.GetTvEpisodeDetails(seriesId, season, episode, language);

                if (_useCaching)
                {
                    _getTvEpisodeDetailsCache.Add(hash, tvEpisode);
                }
            }

            return tvEpisode;
        }
    }
}
