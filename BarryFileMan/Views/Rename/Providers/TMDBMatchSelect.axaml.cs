using Avalonia.Controls;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.ViewModels.Rename.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarryFileMan.Views.Rename.Providers
{
    public partial class TMDBMatchSelect : Window
    {
        TMDBMatchSelectViewModel? _viewModel;
        
        public TMDBMatchSelect()
        {
            InitializeComponent();
        }

        public TMDBMatchSelect(TMDBMatchSelectViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        public static Task<IRenameMatch?> ShowAsync(IEnumerable<IRenameMatch>? matches, Window? parent = null, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
        {
            var promptWindow = new TMDBMatchSelect(new(matches))
            {
                WindowStartupLocation = windowStartupLocation,
            };

            var tcs = new TaskCompletionSource<IRenameMatch?>();
            promptWindow.Closed += delegate { tcs.TrySetResult(promptWindow._viewModel!.SelectedMatch?.RenameMatch); };

            if (parent != null)
                promptWindow.ShowDialog(parent);
            else
                promptWindow.Show();

            return tcs.Task;
        }
    }
}
