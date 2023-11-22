using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Settings
{
    public abstract partial class BaseUserConfigViewModel<T> : ObservableObject
    {
        public abstract T UndoChanges();
        public abstract T ApplyChanges();
    }
}
