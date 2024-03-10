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
        public static string AppDescription => AboutViewModel.AppDescription;
        public static Uri AppUri => AboutViewModel.AppUri;
        public static IEnumerable<PageViewModel> Pages => new List<PageViewModel>()
        {
            new("Rename", "Rename files using various match providers", HelpSections.Rename),
            new("Flatten", "Flatten a directory by moving all the files from subdirectories to the root directory", HelpSections.Flatten)
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
