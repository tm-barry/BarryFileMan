using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
            ThemeHelper.ChangeTheme(AppManager.UserConfig.Config.General.Theme);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow = mainWindow;
                AppManager.Init(mainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}