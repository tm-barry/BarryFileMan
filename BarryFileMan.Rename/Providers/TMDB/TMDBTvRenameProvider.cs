using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBTvRenameProvider : BaseRenameProvider<TMDBRenameProviderMatchOptions>
    {
        public TMDBTvRenameProvider() : base(RenameProviderTypes.TMDB_TV) { }

        public override IEnumerable<IRenameMatch>? Match(string input, TMDBRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IRenameMatch>?> MatchAsync(string input, TMDBRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }
    }
}
