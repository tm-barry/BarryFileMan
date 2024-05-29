using BarryFileMan.Rename.Models;

namespace BarryFileMan.Rename.Interfaces
{
    public interface IRenameProvider
    {
        RenameResult Rename(IEnumerable<IRenameMatch> matches, string renamePattern, string? defaultTagFallbackValue = null);
    }
}
