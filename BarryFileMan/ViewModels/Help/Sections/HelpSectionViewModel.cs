using BarryFileMan.Enums.Help;
using BarryFileMan.ViewModels.About;
using System;

namespace BarryFileMan.ViewModels.Help.Sections
{
    public class HelpSectionViewModel : BaseSectionViewModel
    {
        public override HelpSections Section => HelpSections.Help;
        public static Uri AppUri => AboutViewModel.AppUri;
    }

    public class PageViewModel
    {
        public string Name { get; }
        public string Description { get; }
        public HelpSections Section { get; }

        public PageViewModel(string name, string description, HelpSections section)
        {
            Name = name;
            Description = description;
            Section = section;
        }
    }
}
