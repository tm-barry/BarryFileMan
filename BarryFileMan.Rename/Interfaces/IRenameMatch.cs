namespace BarryFileMan.Rename.Interfaces
{
    public interface IRenameMatch
    {
        Dictionary<string, IList<IRenameMatchGroupValue>> Groups { get; }
    }
}
