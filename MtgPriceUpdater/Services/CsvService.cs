using MtgPriceUpdater.Models;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater.Services
{
    /// <summary>
    /// Service responsible for loading and saving card data to and from CSV files
    /// located in the 'prices' directory.
    /// </summary>
    public class CsvService
    {
        /// <summary>
        /// Loads cards from a CSV file and converts each line into a Card object.
        /// </summary>
        /// <param name="fileName">The CSV filename (e.g., "setname.csv") to load from the price's directory.</param>
        /// <returns>A list of <see cref="Card"/> objects read from the CSV file.</returns>
        public List<Card> LoadFromCsv(string fileName)
        {
            string setsDirectory = DirectoryHelper.GetPricesDirectory();
            string filePath = Path.Combine(setsDirectory, fileName);

            var cards = new List<Card>();

            // If file doesn't exist, return empty list
            if (!File.Exists(filePath))
            {
                Logger.Log($"CSV file not found: {filePath}");
                return cards;
            }

            Logger.Log($"Reading CSV file: {filePath}");

            using var reader = new StreamReader(filePath);
            int lineNumber = 0;

            while (!reader.EndOfStream)
            {
                lineNumber++;
                string? line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');

                // Each card row must contain at least 9 fields
                if (parts.Length < 9)
                {
                    Logger.Log($"Skipping malformed line {lineNumber} in {fileName}: Not enough fields");
                    continue;
                }

                string fullCode = parts[0].Trim(); // Example: "TDM 321F"
                var codeParts = fullCode.Split(' ');

                // Card code should be in the format "SET COLLECTORCODE"
                if (codeParts.Length < 2)
                {
                    Logger.Log($"Skipping invalid card code on line {lineNumber}: {fullCode}");
                    continue;
                }

                string setCode = codeParts[0];
                string collectorRaw = codeParts[1];
                bool isFoil = collectorRaw.EndsWith("F");
                string collectorCode = collectorRaw.TrimEnd('F');

                // Create a Card instance and populate from CSV
                cards.Add(new Card
                {
                    SetCode = setCode,
                    CollectorCode = collectorCode,
                    Name = parts[1].Trim('"'),
                    Rarity = "Unknown", // Can be refined later if available
                    Price = parts[8].Trim(),
                    IsFoil = isFoil
                });
            }

            Logger.Log($"Loaded {cards.Count} cards from {filePath}");
            return cards;
        }

        /// <summary>
        /// Saves a list of cards to a CSV file in the 'prices' directory.
        /// </summary>
        /// <param name="theCards">The list of <see cref="Card"/> objects to save.</param>
        /// <param name="fileName">The target CSV filename (e.g., "setname.csv").</param>
        public void SaveToCsv(List<Card> theCards, string fileName)
        {
            string setsDirectory = DirectoryHelper.GetPricesDirectory();
            string filePath = Path.Combine(setsDirectory, fileName);

            Logger.Log($"Writing to CSV: {filePath}");

            using var writer = new StreamWriter(filePath);

            // Format: item code, name, BLANK , 0, old price, 0, 0, 0, new price
            foreach (var card in theCards)
            {
                // Ensure names with commas are wrapped in quotes
                string formattedName = card.Name.Contains(",") ? $"\"{card.Name}\"" : card.Name;

                writer.WriteLine($"{card.ItemCode},{formattedName},,0,0.0,0,0,0,{card.Price}");
            }

            Logger.Log($"Saved {theCards.Count} cards to {filePath}");
        }
    }
}
