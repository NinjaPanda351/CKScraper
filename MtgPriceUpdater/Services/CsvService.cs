using Microsoft.VisualBasic.FileIO;
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

            if (!File.Exists(filePath))
            {
                Logger.Log($"CSV file not found: {filePath}");
                return cards;
            }

            Logger.Log($"Reading CSV file: {filePath}");

            int lineNumber = 0;

            using var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            while (!parser.EndOfData)
            {
                lineNumber++;
                string[]? parts = parser.ReadFields();
                if (parts is null || parts.Length < 9)
                {
                    Logger.Log($"Skipping malformed line {lineNumber} in {fileName}: Not enough fields");
                    continue;
                }

                string fullCode = parts[0].Trim();
                var codeParts = fullCode.Split(' ');

                if (codeParts.Length < 2)
                {
                    Logger.Log($"Skipping invalid card code on line {lineNumber}: {fullCode}");
                    continue;
                }

                string setCode = codeParts[0];
                string collectorRaw = codeParts[1];
                bool isFoil = collectorRaw.EndsWith("F");
                string collectorCode = collectorRaw.TrimEnd('F');

                cards.Add(new Card
                {
                    SetCode = setCode,
                    CollectorCode = collectorCode,
                    Name = parts[1], // No need to trim quotes — parser handles it
                    Rarity = "Unknown",
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
                string formattedName = EscapeCsvField(card.Name);

                writer.WriteLine($"{card.ItemCode},{formattedName},,0,0.0,0,0,0,{card.Price}");
            }

            Logger.Log($"Saved {theCards.Count} cards to {filePath}");
        }
        
        
        /// <summary>
        /// Escapes a string for safe use in a CSV file.
        /// If the string contains quotes, they are escaped by doubling them.
        /// If the string contains commas or quotes, the entire string is wrapped in double quotes.
        /// </summary>
        /// <param name="value">The field value to escape.</param>
        /// <returns>A CSV-safe version of the input string.</returns>
        private static string EscapeCsvField(string value)
        {
            if (value.Contains('"'))
            {
                value = value.Replace("\"", "\"\""); // Escape embedded quotes
            }

            if (value.Contains(',') || value.Contains('"'))
            {
                value = $"\"{value}\""; // Wrap in quotes if necessary
            }

            return value;
        }

    }
}
