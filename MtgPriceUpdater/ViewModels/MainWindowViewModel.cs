using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using MtgPriceUpdater.Models;
using MtgPriceUpdater.Services;
using MtgPriceUpdater.Utils;
using ReactiveUI;

namespace MtgPriceUpdater.ViewModels
{
    /// <summary>
    /// The ViewModel for the main window.
    /// Manages the list of MTG sets and commands for running updates.
    /// </summary>
    public class MainWindowViewModel
    {
        /// <summary>
        /// Collection of all available set names loaded from set_codes.csv.
        /// </summary>
        public ObservableCollection<SetItem> Sets { get; } = new();

        /// <summary>
        /// Command that saves selected sets and runs the price updater.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RunCommand { get; }

        /// <summary>
        /// Command that selects all sets, saves them, and runs the updater.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RunAllCommand { get; }

        /// <summary>
        /// Initializes the ViewModel by loading set names and setting up commands.
        /// </summary>
        public MainWindowViewModel()
        {
            Logger.Log("[MainWindowViewModel] Initializing ViewModel...");

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

                Logger.Log($"[MainWindowViewModel] Loaded {Sets.Count} sets from {setsFilePath}");
            }
            else
            {
                Logger.Log($"[MainWindowViewModel] File not found: {setsFilePath}");
            }

            RunCommand = ReactiveCommand.CreateFromTask(RunScraperAsync);
            RunAllCommand = ReactiveCommand.CreateFromTask(RunAllAsync);

            Logger.Log("[MainWindowViewModel] ViewModel initialized.");
        }

        /// <summary>
        /// Saves the selected sets to update_sets.txt.
        /// </summary>
        private void SaveSelectedSets()
        {
            string updatePath = Path.Combine(DirectoryHelper.GetDataDirectory(), "update_sets.txt");
            var selected = Sets.Where(s => s.IsSelected).Select(s => s.Name).ToList();

            File.WriteAllLines(updatePath, selected);
            Logger.Log($"[MainWindowViewModel] Saved {selected.Count} sets to update_sets.txt");
        }

        /// <summary>
        /// Runs the price updater on selected sets.
        /// </summary>
        private async Task RunScraperAsync()
        {
            Logger.Log("[MainWindowViewModel] Running price updater for selected sets...");
            SaveSelectedSets();

            var processor = new SetProcessorService();
            await processor.RunAsync();

            Logger.Log("[MainWindowViewModel] Scraping completed.");
        }

        /// <summary>
        /// Selects all sets, saves them, and runs the price updater.
        /// </summary>
        private async Task RunAllAsync()
        {
            Logger.Log("[MainWindowViewModel] Selecting all sets for update...");

            foreach (var set in Sets)
                set.IsSelected = true;

            SaveSelectedSets();

            Logger.Log("[MainWindowViewModel] Running price updater for ALL sets...");
            var processor = new SetProcessorService();
            await processor.RunAsync();

            Logger.Log("[MainWindowViewModel] Scraping for all sets completed.");
        }
    }
}
