using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using BarryFileMan.Rename.Constants;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RenameMatchNodeViewModel : ObservableObject
    {
        public RenameMatchNodeType Type { get; private set; }
        public int MatchIndex { get; private set; }
        public int? GroupIndex { get; private set; }
        public string? Name { get; private set; }
        public string? Value { get; private set; }
        public ObservableCollection<RenameMatchNodeViewModel>? SubNodes { get; }
        public bool HasSubNodes => SubNodes != null && SubNodes.Any();
        public int? GroupKey { get; private set; }
        public bool HasValue => Value != null;

        public string DisplayName
        {
            get
            {
                if (GroupIndex.HasValue && !GroupTags.ReservedTags.Contains(Name ?? ""))
                {
                    return $"{Name}[{GroupIndex}]";
                }
                else if(GroupIndex.HasValue)
                {
                    return $"{Name}";
                }
                else
                {
                    return $"Match{{{MatchIndex}}}";
                }
            }
        }

        public string Icon
        {
            get
            {
                return Type switch
                {
                    RenameMatchNodeType.Match => "group",
                    _ => "tag",
                };
            }
        }

        [ObservableProperty]
        private bool _isExpanded;

        [ObservableProperty]
        private bool _isVisible;

        public RenameMatchNodeViewModel(RenameMatchNodeType type, int matchIndex, ObservableCollection<RenameMatchNodeViewModel>? subNodes = null, 
            int? groupIndex = null, string? value = null, string? name = null, int? groupKey = null, bool isExpanded = false, bool isVisible = true)
        {
            Type = type;
            MatchIndex = matchIndex;
            Name = name;
            SubNodes = subNodes;
            GroupIndex = groupIndex;
            Value = value;
            GroupKey = groupKey;
            IsExpanded = isExpanded;
            IsVisible = isVisible;
        }
    }

    public enum RenameMatchNodeType
    {
        Match,
        Tag
    }
}
