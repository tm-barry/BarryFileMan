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
}
