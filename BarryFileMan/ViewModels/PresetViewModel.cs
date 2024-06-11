using BarryFileMan.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels
{
    public partial class PresetViewModel<T> : ObservableObject, IPresetViewModel
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private bool _isSystem;

        [ObservableProperty]
        private T _preset;

        public PresetViewModel(string name, bool isSystem, T preset)
        {
            Name = name;
            IsSystem = isSystem;
            Preset = preset;
        }
    }
}
