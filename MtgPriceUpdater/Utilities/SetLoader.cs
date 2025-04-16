namespace MtgPriceUpdater.Utils;

/// <summary>
/// Responsible for loading Magic: The Gathering set names and their codes from files.
/// </summary>
public class SetLoader
{
    /// <summary>
    /// Loads a list of set names from a given text file.
    /// </summary>
    /// <param name="filePath">The full path to the file containing the set names.</param>
    /// <returns>A list of set names.</returns>
    public List<string> LoadSetNames(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Logger.Log($"Set list file not found: {filePath}");
            return new List<string>();
        }

        var setNames = new List<string>();
        foreach (var line in File.ReadAllLines(filePath))
        {
            if (!string.IsNullOrWhiteSpace(line))
                setNames.Add(line.Trim());
        }

        Logger.Log($"Loaded {setNames.Count} sets from {filePath}");
        return setNames;
    }

    /// <summary>
    /// Loads a dictionary mapping set names to their corresponding set codes.
    /// </summary>
    /// <param name="filePath">The full path to the CSV file (format: 'SetName,SetCode').</param>
    /// <returns>A dictionary with readable set names as keys and set codes as values.</returns>
    public Dictionary<string, string> LoadSetCodeMap(string filePath)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains(',')) continue;

            var parts = line.Split(',');
            if (parts.Length == 2)
            {
                string readableName = parts[0].Trim();
                string code = parts[1].Trim();
                map[readableName] = code;
            }
            else
            {
                Logger.Log($"Skipping invalid line: {line}");
            }
        }

        Logger.Log($"Loaded {map.Count} set codes from {filePath}");
        return map;
    }
}
