using Avalonia.Platform;
using BarryFileMan.Enums.Help;
using BarryFileMan.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Help
{
    public partial class HelpViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentSection))]
        private ObservableCollection<HelpSections> _sections = new() { };

        public HelpSections CurrentSection => Sections.LastOrDefault();

        [ObservableProperty]
        private string? _markdownText = null;

        public HelpViewModel() : this(HelpSections.Help) { }
        public HelpViewModel(HelpSections section)
        {
            Sections.CollectionChanged += Sections_CollectionChanged;
            Sections.Add(HelpSections.Help);

            if(!Sections.Contains(section))
            {
                Sections.Add(section);
            }
        }

        ~HelpViewModel()
        {
            Sections.CollectionChanged -= Sections_CollectionChanged;
        }

        private void Sections_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CurrentSection));
            GoBackCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanGoBack))]
        private void GoBack()
        {
            if (CanGoBack())
            {
                Sections.RemoveAt(Sections.Count - 1);
            }
        }

        private bool CanGoBack()
        {
            return Sections.Count > 1;
        }

        [RelayCommand]
        private void GoHome()
        {
            if(CurrentSection != HelpSections.Help)
            {
                Sections.Add(HelpSections.Help);
            }
        }
    }
}
