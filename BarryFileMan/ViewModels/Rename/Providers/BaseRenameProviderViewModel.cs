using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class BaseRenameProviderViewModel : ViewModelBase
    {
        public RenameViewModel ViewModel { get; private set; }

        public BaseRenameProviderViewModel(RenameViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
