using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Models.TMDB;

namespace BarryFileMan.Rename.Providers.TMDB
{
    public class TMDBTvRenameMatchProvider : BaseRenameMatchProvider<TMDBTvRenameProviderMatchOptions>
    {
        public TMDBTvRenameMatchProvider() : base(RenameProviderTypes.TMDB_TV) { }

        public override IEnumerable<IRenameMatch>? Match(TMDBTvRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IRenameMatch>?> MatchAsync(TMDBTvRenameProviderMatchOptions? options)
        {
            throw new NotImplementedException();
        }
    }
}
