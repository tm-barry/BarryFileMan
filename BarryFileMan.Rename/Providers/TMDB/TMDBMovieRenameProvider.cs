using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;
using BarryFileMan.Rename.Repositories;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBMovieRenameProvider : BaseRenameProvider<TMDBMovieTvRenameProviderMatchOptions>
    {
        public TMDBMovieRenameProvider() : base(RenameProviderTypes.TMDB_Movie) { }

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
            var tmdbRepo = new TMDBRepository(options.APIKey);
            var movieMatches = await tmdbRepo.SearchMovie(options.Query, options.IncludeAdult, options.Language, null, null, null, options.Year);
            if(movieMatches?.Results != null && movieMatches.Results.Any())
            {
                renameMatches = new List<TMDBRenameMatch>();
                foreach(var movieMatch in movieMatches.Results)
                {
                    var renameMatch = new TMDBRenameMatch();

                    renameMatch.Groups.Add("tmdb-name", 
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.Name) });
                    renameMatch.Groups.Add("tmdb-original-name",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.OriginalName) });
                    renameMatch.Groups.Add("tmdb-original-language",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.OriginalLanguage) });
                    renameMatch.Groups.Add("tmdb-release-date",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.ReleaseDate) });
                    if (DateTime.TryParse(movieMatch.ReleaseDate, out var releaseDate))
                    {
                        renameMatch.Groups.Add("tmdb-release-year",
                            new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(releaseDate.Year.ToString()) });
                    }
                    renameMatch.Groups.Add("tmdb-overview",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.Overview) });
                    renameMatch.Groups.Add("tmdb-poster-path",
                        new List<IRenameMatchGroupValue>() { new TMDBRenameMatchGroupValue(movieMatch.PosterPath) });

                    renameMatches.Add(renameMatch);
                }
            }

            return renameMatches;
        }
    }
}
