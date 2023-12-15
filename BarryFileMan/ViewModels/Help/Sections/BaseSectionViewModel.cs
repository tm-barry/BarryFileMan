using BarryFileMan.Enums.Help;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Help.Sections
{
    public abstract class BaseSectionViewModel : ObservableObject
    {
        public abstract HelpSections Section { get; }
    }
}
