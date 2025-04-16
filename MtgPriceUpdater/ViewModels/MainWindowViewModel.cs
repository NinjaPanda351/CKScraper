using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using MtgPriceUpdater.Models;
using MtgPriceUpdater.Services;
using MtgPriceUpdater.Utils;
using ReactiveUI;

namespace MtgPriceUpdater.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<SetItem> Sets { get; } = new();
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public ReactiveCommand<Unit, Unit> RunAllCommand { get; }


        public MainWindowViewModel()
        {
            string setsFilePath = Path.Combine(DirectoryHelper.GetDataDirectory(), "set_codes.csv");

            if (File.Exists(setsFilePath))
            {
                foreach (var line in File.ReadAllLines(setsFilePath))
                {
                    if (!string.IsNullOrWhiteSpace(line) && line.Contains(','))
                    {
                        var name = line.Split(',')[0].Trim();
                        Sets.Add(new SetItem(name));
                    }
                }
            }
            
            RunCommand = ReactiveCommand.CreateFromTask(RunScraperAsync);
            RunAllCommand = ReactiveCommand.CreateFromTask(RunAllAsync);

        }

        private void SaveSelectedSets()
        {
            string updatePath = Path.Combine(DirectoryHelper.GetDataDirectory(), "update_sets.txt");
            var selected = Sets.Where(s => s.IsSelected).Select(s => s.Name);
            File.WriteAllLines(updatePath, selected);
            Logger.Log($"Saved {selected.Count()} sets to update_sets.txt");
        }
        
        private async Task RunScraperAsync()
        {
            SaveSelectedSets();
            
            Logger.Log("Running MTG Price Updater...");
            var processor = new SetProcessorService();
            await processor.RunAsync();
            Logger.Log("Scraping completed.");
        }
        
        private async Task RunAllAsync()
        {
            // Select all sets
            foreach (var set in Sets)
            {
                set.IsSelected = true;
            }

            SaveSelectedSets(); // Save all to update_sets.txt
            Logger.Log("All sets selected. Starting scraper...");

            var processor = new SetProcessorService();
            await processor.RunAsync();
    
            Logger.Log("Finished running scraper on all sets.");
        }

    }
}