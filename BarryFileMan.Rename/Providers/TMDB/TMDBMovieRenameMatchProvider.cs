using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Extensions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;
using BarryFileMan.Rename.Repositories;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBMovieRenameMatchProvider : BaseRenameMatchProvider<TMDBMovieTvRenameProviderMatchOptions>
    {
        private bool _useCaching;
        private static Dictionary<int, TMDBSearchMovieTV?> _searchMovieCache = new();

        public TMDBMovieRenameMatchProvider(bool useCaching = false) : base(RenameProviderTypes.TMDB_Movie) 
        {
            _useCaching = useCaching;
        }

        public override IEnumerable<IRenameMatch>? Match(TMDBMovieTvRenameProviderMatchOptions? options)
        {
            return Task.Run(() => MatchAsync(options)).GetAwaiter().GetResult();
        }

        public override async Task<IEnumerable<IRenameMatch>?> MatchAsync(TMDBMovieTvRenameProviderMatchOptions? options)
        {
            if(string.IsNullOrWhiteSpace(options?.APIKey))
            {
                throw new TMDBBadResponseException("Invalid API key: You must be granted a valid key.", -2);
            }

            if(string.IsNullOrWhiteSpace(options?.Query))
            {
                throw new TMDBBadResponseException("Invalid Query: Query must not be blank.", -3);
            }

            List<TMDBRenameMatch>? renameMatches = null;
            var movieMatches = await SearchMovie(options);

            if(movieMatches?.Results != null && movieMatches.Results.Any())
            {
                renameMatches = new List<TMDBRenameMatch>();
                foreach(var movieMatch in movieMatches.Results)
                {
                    var renameMatch = new TMDBRenameMatch();

                    renameMatch.Groups.Add("tmdbName", 
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.Name) });
                    renameMatch.Groups.Add("tmdbOriginalName",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.OriginalName) });
                    renameMatch.Groups.Add("tmdbOriginalLanguage",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.OriginalLanguage) });
                    renameMatch.Groups.Add("tmdbReleaseDate",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.ReleaseDate) });
                    if (DateTime.TryParse(movieMatch.ReleaseDate, out var releaseDate))
                    {
                        renameMatch.Groups.Add("tmdbReleaseYear",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(releaseDate.Year.ToString()) });
                    }
                    renameMatch.Groups.Add("tmdbOverview",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.Overview) });
                    renameMatch.Groups.Add("tmdbPosterPath",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.PosterPath) });

                    renameMatches.Add(renameMatch);
                }
            }

            return renameMatches;
        }

        private async Task<TMDBSearchMovieTV?> SearchMovie(TMDBMovieTvRenameProviderMatchOptions options)
        {
            TMDBSearchMovieTV? movieMatches = null;
            var tmdbRepo = new TMDBRepository(options.APIKey);
            var query = options.Query.ToLower().Trim();
            var language = options.Language.ToLower().Trim();
            var year = options.Year.ToLower().Trim();
            var hash = _useCaching ? new[] { query, options.IncludeAdult.ToString(), language, year }.GetSequenceHashCode() : 0;

            if (_useCaching && _searchMovieCache.ContainsKey(hash))
            {
                movieMatches = _searchMovieCache[hash];
            }
            else
            {
                movieMatches = await tmdbRepo.SearchMovie(options.Query, options.IncludeAdult, language, null, null, null, year);

                if (_useCaching)
                {
                    _searchMovieCache.Add(hash, movieMatches);
                }
            }

            return movieMatches;
        }
    }
}
