using BarryFileMan.Enums.Help;
using System;

namespace BarryFileMan.ViewModels.Help.Sections
{
    public class RenameTMDBSectionViewModel : BaseSectionViewModel
    {
        public override HelpSections Section => HelpSections.RenameTMDB;
        public static Uri TmdbAPIDocumentationUri => new(Resources.Resources.TmdbAPIDocumentationUri);
    }
}
