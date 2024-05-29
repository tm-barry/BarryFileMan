using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Models;

namespace BarryFileMan.Rename.Interfaces
{
    public interface IRenameMatchProvider<TMatchOptions> : IRenameProvider where TMatchOptions : class
    {
        RenameProviderTypes ProviderType { get; }

        IEnumerable<IRenameMatch>? Match(TMatchOptions? options);
        Task<IEnumerable<IRenameMatch>?> MatchAsync(TMatchOptions? options);
    }
}
