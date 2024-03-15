using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels
{
    public partial class NameDescriptionViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _description;

        public NameDescriptionViewModel() { }
        public NameDescriptionViewModel(string? name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
