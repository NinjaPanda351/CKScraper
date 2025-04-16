using Avalonia.Controls;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater.Views
{
    /// <summary>
    /// The main window of the MTG Price Updater application.
    /// Provides the user interface for set selection and actions.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Loads the associated XAML UI components.
        /// </summary>
        public MainWindow()
        {
            Logger.Log("[MainWindow] Initializing main window...");
            InitializeComponent();
            Logger.Log("[MainWindow] Main window initialized.");
        }
    }
}