namespace MtgPriceUpdater.Utils
{
    /// <summary>
    /// Provides utility methods for accessing project directory paths
    /// and ensuring required folders exist.
    /// </summary>
    public static class DirectoryHelper
    {
        // Root directory relative to bin path (typically three levels up during debugging)
        private static readonly string ProgramDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        
        /// <summary>
        /// Gets the root directory of the project based on the current executing context.
        /// </summary>
        /// <returns>The full path to the project root directory.</returns>
        public static string GetProgramDirectory()
        {
            return ProgramDirectory;
        }

        /// <summary>
        /// Gets the full path to the "Data" directory and ensures it exists.
        /// </summary>
        /// <returns>The full path to the Data directory.</returns>
        public static string GetDataDirectory()
        {
            string dataDirectory = Path.Combine(ProgramDirectory, "data");
            EnsureDirectoryExists(dataDirectory);
            return dataDirectory;
        }

        /// <summary>
        /// Gets the full path to the "Data/prices" directory and ensures it exists.
        /// </summary>
        /// <returns>The full path to the price's directory.</returns>
        public static string GetPricesDirectory()
        {
            string pricesDirectory = Path.Combine(GetDataDirectory(), "prices");
            EnsureDirectoryExists(pricesDirectory);
            return pricesDirectory;
        }

        /// <summary>
        /// Ensures that the specified directory exists, creating it if necessary.
        /// Logs directory creation.
        /// </summary>
        /// <param name="directory">The path of the directory to verify or create.</param>
        private static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Logger.Log($"[DirectoryHelper] Created missing directory: {directory}");
            }
        }
    }
}
