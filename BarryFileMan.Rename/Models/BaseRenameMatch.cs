using BarryFileMan.Rename.Interfaces;

namespace BarryFileMan.Rename.Models
{
    public class BaseRenameMatch : IRenameMatch
    {
        public Dictionary<string, IList<IRenameMatchGroupValue>> Groups { get; }

        public BaseRenameMatch() : this(new Dictionary<string, IList<IRenameMatchGroupValue>>()) { }

        public BaseRenameMatch(Dictionary<string, IList<IRenameMatchGroupValue>> groups)
        {
            Groups = groups;
        }
    }
}
