using MtgPriceUpdater.Services;
using MtgPriceUpdater.Utils;

namespace MtgPriceUpdater;

/// <summary>
/// Entry point for the MTG Price Updater application.
/// Initializes and runs the SetProcessorService to scrape and update MTG card prices.
/// </summary>
class Program
{
    /// <summary>
    /// Main method. Runs the update process asynchronously.
    /// </summary>
    /// <param name="args">Command-line arguments (currently unused).</param>
    static async Task Main(string[] args)
    {
        Logger.Log("========== MTG Price Updater Started ==========");

        try
        {
            var processor = new SetProcessorService();
            await processor.RunAsync();
        }
        catch (Exception ex)
        {
            Logger.Log($"Unhandled error in Main: {ex.Message}");
        }

        Logger.Log("========== MTG Price Updater Finished ==========");
    }
}