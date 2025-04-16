using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MtgPriceUpdater.Views;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater
{
    /// <summary>
    /// Represents the entry point for the Avalonia application.
    /// Handles initialization and sets the main window.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes the application and loads XAML definitions.
        /// </summary>
        public override void Initialize()
        {
            Logger.Log("[App] Initializing Avalonia application...");
            AvaloniaXamlLoader.Load(this);
            Logger.Log("[App] AvaloniaXamlLoader completed.");
        }

        /// <summary>
        /// Called when the Avalonia framework has finished initializing.
        /// Sets the main window for classic desktop-style applications.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            Logger.Log("[App] Framework initialization completed.");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Logger.Log("[App] Setting MainWindow...");
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}