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
