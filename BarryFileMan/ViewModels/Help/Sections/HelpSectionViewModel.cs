using BarryFileMan.Enums.Help;
using BarryFileMan.ViewModels.About;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Help.Sections
{
    public class HelpSectionViewModel : BaseSectionViewModel
    {
        public override HelpSections Section => HelpSections.Help;
        public static Uri AppUri => AboutViewModel.AppUri;
        public static IEnumerable<PageViewModel> Pages => new List<PageViewModel>()
        {
            new(Resources.Resources.Rename, Resources.Resources.RenamePageDescription, HelpSections.Rename),
            new(Resources.Resources.Flatten, Resources.Resources.FlattenPageDescription, HelpSections.Flatten)
        };
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
