using BarryFileMan.Enums.Help;
using BarryFileMan.ViewModels.About;
using System;

namespace BarryFileMan.ViewModels.Help.Sections
{
    public class RenameRegexSectionViewModel : BaseSectionViewModel
    {
        public override HelpSections Section => HelpSections.RenameRegex;
        public static Uri DotNetRegexQuickReferenceUri => new(Resources.Resources.DotNetRegexQuickReferenceUrl);
    }
}
