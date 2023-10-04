using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BarryFileMan.Helpers;
using BarryFileMan.Managers;
using BarryFileMan.ViewModels;
using BarryFileMan.Views;

namespace BarryFileMan
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            if (Application.Current != null)
                Application.Current.RequestedThemeVariant = ThemeHelper.GetThemeVariant(AppManager.UserConfig.Config.Theme);

            base.OnFrameworkInitializationCompleted();
        }
    }
}