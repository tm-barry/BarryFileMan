using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBMovieRenameProvider : BaseRenameProvider<TMDBRenameProviderMatchOptions>
    {
        public TMDBMovieRenameProvider() : base(RenameProviderTypes.TMDB_Movie) { }

        public override IEnumerable<IRenameMatch>? Match(TMDBRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IRenameMatch>?> MatchAsync(TMDBRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }
    }
}
