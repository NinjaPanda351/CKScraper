using MtgPriceUpdater.Models;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater.Services;

/// <summary>
/// Orchestrates the loading, scraping, and saving of MTG card price data for all sets.
/// </summary>
public class SetProcessorService
{
    private readonly CsvService _csvService;
    private readonly ScraperService _scraperService;
    private readonly SetLoader _setLoader;
    private readonly HttpClient _httpClient;

    private readonly string _combinedCsvPath =
        Path.Combine(DirectoryHelper.GetPricesDirectory(), "00_combined_list_changes.csv");

    // Delay ranges to avoid hitting rate limits
    private const int MinPageDelay = 1250;
    private const int MaxPageDelay = 2000;
    private const int MinSetDelay = 6000;
    private const int MaxSetDelay = 10000;

    /// <summary>
    /// Initializes required services. This constructor avoids dependency injection for simplicity.
    /// </summary>
    public SetProcessorService()
    {
        _csvService = new CsvService();
        _scraperService = new ScraperService();
        _setLoader = new SetLoader();
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Main method that loops through all sets, scrapes data, updates prices, and saves to CSV.
    /// </summary>
    public async Task RunAsync()
    {
        File.WriteAllText(_combinedCsvPath, string.Empty);
        Logger.Log("Initialized 00__combined_list_changes.csv");
        
        string progressPath = Path.Combine(DirectoryHelper.GetDataDirectory(), "progress.txt");

        // Resume from a previous failure if a checkpoint exists
        string resumeFrom = File.Exists(progressPath) ? File.ReadAllText(progressPath).Trim() : string.Empty;
        bool skipping = !string.IsNullOrEmpty(resumeFrom);

        var setNames = _setLoader.LoadSetNames(Path.Combine(DirectoryHelper.GetDataDirectory(), "update_sets.txt"));
        var codeMap = _setLoader.LoadSetCodeMap(Path.Combine(DirectoryHelper.GetDataDirectory(), "set_codes.csv"));

        if (!setNames.Any())
        {
            Logger.Log("No sets found. Exiting.");
            return;
        }

        foreach (var setName in setNames)
        {
            if (skipping)
            {
                if (setName == resumeFrom)
                    skipping = false;
                else
                {
                    Logger.Log($"Skipping {setName}...");
                    continue;
                }
            }

            File.WriteAllText(progressPath, setName);

            if (!codeMap.TryGetValue(setName, out var setCode))
            {
                Logger.Log($"No set code for {setName}. Skipping.");
                continue;
            }

            Logger.Log($"Processing {setName}...");
            string baseUrl = $"https://www.cardkingdom.com/mtg/{setName}";
            string csvFilePath = Path.Combine(DirectoryHelper.GetPricesDirectory(), $"{setName}.csv");

            var existingCards = _csvService.LoadFromCsv(csvFilePath);
            Logger.Log($"Loaded {existingCards.Count} existing cards for {setName}.");

            try
            {
                int totalCards = await _scraperService.GetTotalCardsAsync(_httpClient, baseUrl);
                if (totalCards == 0)
                {
                    Logger.Log("No cards found on regular page. Skipping set.");
                    continue;
                }

                int pages = (int)Math.Ceiling(totalCards / 25.0);
                var newCards = new List<Card>();

                for (int i = 1; i <= pages; i++)
                {
                    string url = $"{baseUrl}?page={i}";

                    _httpClient.DefaultRequestHeaders.UserAgent.Clear();
                    string userAgent = UserAgentProvider.GetRandomAgent();
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                    Logger.Log($"Fetching page {i}/{pages} with user agent: {userAgent}");

                    newCards.AddRange(await _scraperService.FetchCardsAsync(_httpClient, url, false, setCode));
                    await Task.Delay(Random.Shared.Next(MinPageDelay, MaxPageDelay));
                }

                // Foil version
                Logger.Log("Fetching foils...");
                int totalFoils = await _scraperService.GetTotalCardsAsync(_httpClient, baseUrl + "/foils");

                if (totalFoils > 0)
                {
                    int foilPages = (int) Math.Ceiling(totalFoils / 25.0);
                    for (int i = 1; i <= foilPages; i++)
                    {
                        string foilUrl = $"{baseUrl}/foils?page={i}";

                        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
                        string userAgent = UserAgentProvider.GetRandomAgent();
                        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                        Logger.Log($"Fetching page {i}/{foilPages} with user agent: {userAgent}");

                        newCards.AddRange(await _scraperService.FetchCardsAsync(_httpClient, foilUrl, true, setCode));
                        await Task.Delay(Random.Shared.Next(MinPageDelay, MaxPageDelay));
                    }
                }

                Logger.Log($"Fetched {newCards.Count} total cards (including foils). Updating prices...");
                UpdatePrices(existingCards, newCards);
                _csvService.SaveToCsv(existingCards, csvFilePath);
                AppendCsvToCombined(csvFilePath);

                File.Delete(progressPath); // Remove resume point on successful completion
                Logger.Log($"Finished processing {setName}.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error: {ex.Message}");
                Logger.Log("Processing stopped. Will resume from this set.");
                return;
            }

            await Task.Delay(Random.Shared.Next(MinSetDelay, MaxSetDelay));
        }
    }

    /// <summary>
    /// Updates prices of matching cards and adds new cards to the list.
    /// </summary>
    private void UpdatePrices(List<Card> existing, List<Card> updates)
    {
        int updated = 0;
        foreach (var card in updates)
        {
            var match = existing.FirstOrDefault(c =>
                c.Name == card.Name &&
                c.CollectorCode == card.CollectorCode &&
                c.IsFoil == card.IsFoil);

            if (match != null)
            {
                match.Price = card.Price;
                updated++;
            }
            else
            {
                existing.Add(card);
            }
        }

        Logger.Log($"Updated {updated} prices. Added {updates.Count - updated} new cards.");
    }

    /// <summary>
    /// Appends the data in a CSV file to a shared combined CSV file
    /// </summary>
    /// <param name="sourceFilePath">The file path for the CSV data you want copied</param>
    private void AppendCsvToCombined(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath)) return;

        // Read and append all lines (no header to skip)
        var lines = File.ReadLines(sourceFilePath);
        File.AppendAllLines(_combinedCsvPath, lines);

        Logger.Log($"Appended {lines.Count()} lines from {Path.GetFileName(sourceFilePath)} to combined CSV.");

    }
}
