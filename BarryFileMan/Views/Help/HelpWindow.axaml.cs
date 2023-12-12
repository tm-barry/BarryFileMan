using Avalonia.Controls;
using BarryFileMan.Enums.Help;
using BarryFileMan.ViewModels.Help;
using System.Threading.Tasks;

namespace BarryFileMan;

public partial class HelpWindow : Window
{
    private static HelpWindow? _instance;

    public HelpWindow() : this(new()) { }
    public HelpWindow(HelpViewModel helpViewModel)
    {
        DataContext = helpViewModel;
        InitializeComponent();
    }

    public static Task ShowAsync(HelpSections section, Window? parent = null, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen)
    {
        if (_instance == null)
        {
            var helpViewModel = new HelpViewModel(section);
            _instance = new HelpWindow(helpViewModel)
            {
                WindowStartupLocation = windowStartupLocation,
            };
        }
        else if(_instance.DataContext is HelpViewModel helpViewModel && helpViewModel.CurrentSection != section)
        {
            helpViewModel.Sections.Add(section);
        }

        var tcs = new TaskCompletionSource();
        _instance.Closed += delegate 
        { 
            tcs.TrySetResult();
            _instance = null;
        };

        if (!_instance.IsVisible)
        {
            if (parent != null)
                _instance.ShowDialog(parent);
            else
                _instance.Show();
        }
        else
        {
            _instance.Activate();
        }

        return tcs.Task;
    }
}