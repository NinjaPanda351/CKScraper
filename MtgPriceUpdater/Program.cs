using System;
using Avalonia;
using Avalonia.ReactiveUI;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater
{
    /// <summary>
    /// The main entry point of the MTG Price Updater application.
    /// Initializes Avalonia and launches the GUI using a classic desktop lifetime.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            Logger.Log("Starting Avalonia application...");
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
                Logger.Log("Application closed gracefully.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Fatal error during startup: {ex.Message}");
                Logger.Log("Application terminated unexpectedly.");
            }
        }

        /// <summary>
        /// Configures Avalonia with platform-specific settings and logging.
        /// </summary>
        /// <returns>A configured Avalonia AppBuilder instance.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}