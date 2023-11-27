using CommunityToolkit.Mvvm.ComponentModel;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public abstract partial class BaseRenameProviderViewModel : ObservableValidator
    {
        public RenameViewModel ViewModel { get; private set; }

        public BaseRenameProviderViewModel(RenameViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public abstract void ApplyFileRenames();
    }
}
