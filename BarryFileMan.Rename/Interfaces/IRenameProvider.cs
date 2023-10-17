using BarryFileMan.Rename.Enums;
using BarryFileMan.Rename.Models;

namespace BarryFileMan.Rename.Interfaces
{
    public interface IRenameProvider<TMatchOptions> where TMatchOptions : class
    {
        RenameProviderTypes ProviderType { get; }

        IEnumerable<IRenameMatch>? Match(string input, TMatchOptions? options);
        Task<IEnumerable<IRenameMatch>?> MatchAsync(string input, TMatchOptions? options);
        RenameResult Rename(IEnumerable<IRenameMatch> matches, string renamePattern);
    }
}
