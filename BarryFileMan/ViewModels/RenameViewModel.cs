using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels
{
    public partial class RenameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<string> _files = new();

        [ObservableProperty]
        private string? _selectedFile;

        public RenameViewModel()
        {
            for(int i = 0; i <= 1000; i++)
            {
                Files.Add(i.ToString());
            }
        }
    }
}
