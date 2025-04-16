namespace MtgPriceUpdater.Utilities;

/// <summary>
/// Provides randomized User-Agent strings for simulating diverse browser requests.
/// </summary>
public static class UserAgentProvider
{
    /// <summary>
    /// A pool of common User-Agent strings from various browsers and platforms.
    /// These are rotated to avoid detection or throttling during web scraping.
    /// </summary>
    private static readonly string[] UserAgents =
    [
        //TODO
        //FUTURE IMPROVEMENT: USER AGENTS CAN GO STALE
        //THEREFORE, I NEED TO IMPLEMENT A WAY TO UPDATE USER AGENTS
        //UPDATED USER AGENTS CAN BE SCRAPED FROM THIS WEBSITE
        //https://user-agents.net/
        
        // Chrome (Windows)
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36",

        // Firefox (Windows)
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0",

        // Edge (Windows)
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.2365.80",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.6312.86 Safari/537.36 Edg/123.0.2420.65",

        // Safari (macOS)
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_5) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.5 Safari/605.1.15",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.6 Safari/605.1.15",

        // Chrome (macOS)
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36",

        // Firefox (macOS)
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 13.3; rv:123.0) Gecko/20100101 Firefox/123.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 12.5; rv:122.0) Gecko/20100101 Firefox/122.0",

        // Chrome (Linux)
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
        "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:123.0) Gecko/20100101 Firefox/123.0",

        // iPhone Safari
        "Mozilla/5.0 (iPhone; CPU iPhone OS 17_2_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Mobile/15E148 Safari/604.1",

        // iPad Safari
        "Mozilla/5.0 (iPad; CPU OS 17_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",
        "Mozilla/5.0 (iPad; CPU OS 16_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Mobile/15E148 Safari/604.1",

        // Android Chrome
        "Mozilla/5.0 (Linux; Android 14; Pixel 8 Pro) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; Android 13; SM-G991U) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Mobile Safari/537.36",

        // Android Firefox
        "Mozilla/5.0 (Android 13; Mobile; rv:123.0) Gecko/123.0 Firefox/123.0",
        "Mozilla/5.0 (Android 12; Mobile; rv:122.0) Gecko/122.0 Firefox/122.0",

        // Samsung Internet
        "Mozilla/5.0 (Linux; Android 13; SAMSUNG SM-G998U) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/22.0 Chrome/123.0.0.0 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; Android 12; SAMSUNG SM-G973U) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/21.0 Chrome/122.0.0.0 Mobile Safari/537.36",

        // Opera Mobile
        "Mozilla/5.0 (Linux; Android 13; Pixel 6a) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Mobile Safari/537.36 OPR/74.0.3922.71078",
        "Mozilla/5.0 (Linux; Android 12; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Mobile Safari/537.36 OPR/73.3.3974.71296"
    ];

    /// <summary>
    /// Random number generator for selecting user agents.
    /// </summary>
    private static readonly Random Rng = new();

    /// <summary>
    /// Returns a random User-Agent string from the pool.
    /// </summary>
    /// <returns>A User-Agent string.</returns>
    public static string GetRandomAgent()
    {
        return UserAgents[Rng.Next(UserAgents.Length)];
    }
}
