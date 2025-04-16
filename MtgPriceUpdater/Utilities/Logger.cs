namespace MtgPriceUpdater.Utilities
{
    /// <summary>
    /// A simple logging utility that writes timestamped messages to the console.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logs a message to the console with a timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
        }

        // Future improvement: Add support for logging to a file
        // public static void LogToFile(string message, string logPath) { ... }

        // Future improvement: Add log level filtering (Info, Warn, Error, Debug)
        // public static void Log(string message, LogLevel level) { ... }
    }
}