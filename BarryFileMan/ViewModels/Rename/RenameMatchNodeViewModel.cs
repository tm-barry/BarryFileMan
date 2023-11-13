using System;
using System.Collections.ObjectModel;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public class RenameMatchNodeViewModel
    {
        public int MatchIndex { get; private set; }
        public int? GroupIndex { get; private set; }
        public string? Name { get; private set; }
        public string? Value { get; private set; }
        public ObservableCollection<RenameMatchNodeViewModel>? SubNodes { get; }
        public int? GroupKey { get; private set; }
        public bool HasValue => Value != null;

        public string DisplayName
        {
            get
            {
                if (GroupIndex.HasValue)
                {
                    return $"{Name} ({GroupIndex})";
                }
                else
                {
                    return $"Match {MatchIndex}";
                }
            }
        }

        public RenameMatchNodeViewModel(int matchIndex, ObservableCollection<RenameMatchNodeViewModel>? subNodes = null, 
            int? groupIndex = null, string? value = null, string? name = null, int? groupKey = null)
        {
            MatchIndex = matchIndex;
            Name = name;
            SubNodes = subNodes;
            GroupIndex = groupIndex;
            Value = value;
            GroupKey = groupKey;
        }
    }
}
