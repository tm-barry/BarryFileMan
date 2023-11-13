using BarryFileMan.Rename.Interfaces;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Rename
{
    public class RenameMatchViewModel : ViewModelBase
    {
        public int Index { get; private set; }
        public IList<RenameMatchGroupViewModel> Groups { get; private set; }
        public string Name => $"Match {Index + 1}";

        public RenameMatchViewModel(int index, Dictionary<string, IList<IRenameMatchGroupValue>> groupDictionary)
        {
            Index = index;

            var groups = new List<RenameMatchGroupViewModel>();
            foreach (var groupName in groupDictionary.Keys)
            {
                var groupValues = groupDictionary[groupName];
                for(int i = 0; i < groupValues.Count; i++)
                {
                    groups.Add(new(groupName, i, groupValues[i].Value));
                }
            }
            Groups = groups;
        }
    }

    public class RenameMatchGroupViewModel : ViewModelBase
    {
        public string GroupName { get; private set; }
        public int Index { get; private set; }
        public string Value { get; private set; }
        public string Name => $"{GroupName} ({Index + 1})";

        public RenameMatchGroupViewModel(string groupName, int index, string value)
        {
            GroupName = groupName;
            Index = index;
            Value = value;
        }
    }
}
