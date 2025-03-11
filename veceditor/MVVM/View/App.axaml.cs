using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using veceditor.MVVM.ViewModel;

namespace veceditor.MVVM.View
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
               var viewModel = new MainWindowViewModel();
               var main = new MainWindow(viewModel)
               {
                  DataContext = viewModel
               };
               desktop.MainWindow = main;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}