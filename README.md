# MTG Price Updater

A C# desktop application that scrapes Magic: The Gathering card prices from [Card Kingdom](https://www.cardkingdom.com) and updates local CSV files with current pricing information.

Built with:
- .NET 8
- Avalonia UI (for cross-platform GUI)
- HTMLAgilityPack (for web scraping)
- ReactiveUI (for MVVM bindings)

---

## Features

- ✅ Scrapes prices and availability from Card Kingdom  
- ✅ Supports both regular and foil cards  
- ✅ Automatic price adjustments by card rarity  
- ✅ Saves data to CSVs for integration with POS systems  
- ✅ Tracks progress and resumes from last set if interrupted  
- ✅ GUI for selecting sets to update  
- ✅ "Run All Sets" button to mass-update all known sets  
- ✅ Combined changes CSV (`00__combined_list_changes.csv`) for post-run review  

---

## Project Structure

```text
MtgPriceUpdater/
├── Models/           # Card and SetItem models
├── Services/         # Scraper logic, CSV handler, runner
├── Utils/            # Logging, directory helpers, user-agent rotator
├── Views/            # Avalonia XAML UI files (MainWindow.axaml)
├── ViewModels/       # MVVM ViewModels (MainWindowViewModel)
├── Data/             # Local data (set_codes.csv, update_sets.txt, prices/)
│   └── prices/
│       ├── 3rd-Edition.csv
│       └── ...
├── Program.cs        # Entry point
├── App.axaml         # Avalonia app bootstrapper
├── README.md         # This file
└── MtgPriceUpdater.csproj
```
---

## Data File Expectations

### 'set_codes.csv'
A comma-seperated list of display names and corresponding set codes

### 'update_sets.csv'
Plain text list of set names (one per line) that should be scraped during the next run.
This is managed by the GUI.

---

## How to Run
1. Open the solution in JetBrains Rider or Visual Studio.
2. Make sure the working directory includes the `Data/` folder and valid `set_codes.csv`.
3. Build and run the project.
4. Use the GUI to select desired sets or click **Run All Sets**.
5. Click **Run Scraper** to start scraping.
OPTIONAL: Publish to an Application to run from shortcut

---
## Sample Output

Each `{SetName}.csv` will look like:
3ED 290,Underground Sea,,0,0.0,0,0,0,1080.00

3ED 288,Tropical Island,,0,0.0,0,0,0,630.00

3ED 289,Tundra,,0,0.0,0,0,0,495.00

3ED 291,Volcanic Island,,0,0.0,0,0,0,720.00

3ED 282,Badlands,,0,0.0,0,0,0,495.00


In the form ItemCode, ItemName, BLANK, 0, Old_Price, 0, 0, 0, New_Price

The `00__combined_list_changes.csv` aggregates all updated cards across sets during that session.

---

## Developer Notes

- The scraper uses `HtmlAgilityPack` and `HttpClient` with randomized User-Agent headers from a pool to avoid rate-limiting.
- It includes retry logic for HTTP 429 errors and randomized delay between page and set loads.
- The `DirectoryHelper` ensures required folders are created at runtime.

---

## Future Ideas

- Add support for proxy rotation.
- Visual progress bar for scraper in GUI.
- Export combined CSV as JSON or Excel.
- Schedule recurring updates (e.g., weekly cron-like system).
- Add sorting/filtering UI for card rarity, price delta, etc.

---

## Author

**Joey Jayden Fausto**  
Computer Science and Engineering  
University of Washington  
[joeyfausto107@gmail.com](mailto:joeyfausto107@gmail.com)

---





