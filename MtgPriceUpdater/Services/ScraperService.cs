using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MtgPriceUpdater.Models;
using MtgPriceUpdater.Utilities;

namespace MtgPriceUpdater.Services;

/// <summary>
/// Handles scraping HTML content from Card Kingdom and parsing card data.
/// </summary>
public class ScraperService
{
    /// <summary>
    /// Scrapes a page and determines the total number of card listings available.
    /// </summary>
    /// <param name="client">The HttpClient to perform the request.</param>
    /// <param name="url">The URL of the card listing page.</param>
    /// <returns>Total number of cards on the page, or 0 if not found.</returns>
    public async Task<int> GetTotalCardsAsync(HttpClient client, string url)
    {
        string html = await FetchHtmlAsync(client, url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var resultsText = doc.DocumentNode
            .SelectSingleNode("//div[contains(@class, 'resultsCount')]")
            ?.InnerText;

        if (string.IsNullOrEmpty(resultsText))
        {
            Logger.Log($"No results text found at {url}");
            return 0;
        }

        var match = Regex.Match(resultsText, @"of\s(\d+)\sresults");
        if (!match.Success)
        {
            Logger.Log($"Could not parse total card count from: '{resultsText}'");
            return 0;
        }

        return int.Parse(match.Groups[1].Value);
    }

    /// <summary>
    /// Fetches card data from a specific page and parses HTML into card objects.
    /// </summary>
    /// <param name="client">HttpClient used for the request.</param>
    /// <param name="pageUrl">The full URL of the page to scrape.</param>
    /// <param name="isFoil">Whether to mark these cards as foils.</param>
    /// <param name="setCode">The internal set code to assign to these cards.</param>
    /// <returns>List of Card objects parsed from the HTML.</returns>
    public async Task<List<Card>> FetchCardsAsync(HttpClient client, string pageUrl, bool isFoil, string setCode)
    {
        var cardList = new List<Card>();
        string html = await FetchHtmlAsync(client, pageUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Try to locate all card wrappers in the HTML DOM structure
        var cards = doc.DocumentNode
            .SelectSingleNode("//div[@id='appWrapper']")
            ?.SelectSingleNode(".//div[@id='ckmain' and contains(@class, 'container mainWrapper')]")
            ?.SelectSingleNode(".//div[@id='main' and contains(@class, 'shopMain')]")
            ?.SelectSingleNode(".//div[contains(@class, 'col-sm-9 mainListing')]")
            ?.SelectNodes(".//div[contains(@class, 'productItemWrapper') and contains(@class, 'productCardWrapper')]");

        if (cards == null)
        {
            Logger.Log($"No cards found on page: {pageUrl}");
            return cardList;
        }

        foreach (var card in cards)
        {
            var info = card.SelectSingleNode(".//div[contains(@class, 'itemContentWrapper')]");
            if (info == null) continue;

            var titleNode = info.SelectSingleNode(".//span[contains(@class, 'productDetailTitle')]");
            if (titleNode == null) continue;

            
            var titleLink = titleNode.SelectSingleNode(".//a");
            string cardNameRaw = WebUtility.HtmlDecode(titleLink?.InnerText.Trim() ?? "Unknown");
            string cardName = cardNameRaw.Contains(',') ? $"{cardNameRaw}" : cardNameRaw;

            var collectorNode = info
                .SelectSingleNode(".//div[contains(@class, 'productDetailSet')]")
                ?.SelectSingleNode(".//div[contains(@class, 'collector-number')]");
            if (collectorNode == null) continue;

            string collectorRaw = collectorNode.InnerText;
            string collectorCode = Regex.Match(collectorRaw, @"\d+").Value;
            collectorCode = string.IsNullOrWhiteSpace(collectorCode) ? "Unknown" : int.Parse(collectorCode).ToString();

            var setAndRarityNode = info
                .SelectSingleNode(".//div[contains(@class, 'productDetailSet')]")
                ?.SelectSingleNode(".//a");
            if (setAndRarityNode == null) continue;

            string rarityRaw = setAndRarityNode.InnerText;
            string rarity = Regex.Match(rarityRaw, @"\((.*?)\)").Groups[1].Value.Trim();

            var priceInput = info
                .SelectSingleNode(".//div[contains(@class, 'addToCartWrapper')]")
                ?.SelectSingleNode(".//ul[contains(@class, 'addToCartByType')]")
                ?.SelectSingleNode(".//li[contains(@class, 'EX')]")
                ?.SelectSingleNode(".//input[contains(@name, 'price')]");
            if (priceInput == null) continue;

            string priceText = priceInput.GetAttributeValue("value", "0.00");

            if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal rawPrice))
            {
                Logger.Log($"Invalid price '{priceText}' for card '{cardName}' on {pageUrl}");
                continue;
            }

            decimal adjustedPrice = AdjustPriceByRarity(rawPrice, rarity);


            if (isFoil)
            {
                bool isPresent = false;
                
                foreach (var item in cardList)
                {
                    if (item.CollectorCode == collectorCode)
                    {
                        isPresent = true;
                        break;
                    }
                }

                if (!isPresent)
                {
                    Logger.Log($"Creating duplicate NON-FOIL variant for {setCode} {collectorCode}F - {cardName}");
                    cardList.Add(new Card
                    {
                        Name = cardName,
                        CollectorCode = collectorCode,
                        Rarity = rarity,
                        Price = adjustedPrice.ToString("F2", CultureInfo.InvariantCulture),
                        IsFoil = false,
                        SetCode = setCode
                    });
                }
            }
            
            cardList.Add(new Card
            {
                Name = cardName,
                CollectorCode = collectorCode,
                Rarity = rarity,
                Price = adjustedPrice.ToString("F2", CultureInfo.InvariantCulture),
                IsFoil = isFoil,
                SetCode = setCode
            });
        }

        Logger.Log($"Fetched {cardList.Count} cards from {pageUrl} (isFoil: {isFoil})");
        return cardList;
    }

    /// <summary>
    /// Makes a resilient HTTP GET request and handles rate-limiting.
    /// </summary>
    private async Task<string> FetchHtmlAsync(HttpClient client, string url)
    {
        const int maxRetries = 5;
        const int baseDelay = 10000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException ex) when ((int?)ex.StatusCode == 429)
            {
                int delay = Math.Min(baseDelay * (int)Math.Pow(2, attempt - 1) + Random.Shared.Next(1000, 3000), 180_000);
                Logger.Log($"Rate limit hit (attempt {attempt}). Retrying in {delay}ms...");
                await Task.Delay(delay);
            }
        }

        throw new HttpRequestException("Exceeded retry attempts due to rate limiting.");
    }

    /// <summary>
    /// Applies rounding and minimum price rules based on rarity of the card.
    /// </summary>
    private decimal AdjustPriceByRarity(decimal price, string rarity)
    {
        if (rarity == "C" && price < 0.35m) return 0.25m;
        if (rarity == "UC" && price < 0.35m) return 0.25m;
        if ((rarity == "R" || rarity == "M") && price < 0.50m) return 0.50m;

        return price < 10m
            ? Math.Round(price * 2, MidpointRounding.AwayFromZero) / 2
            : Math.Round(price, MidpointRounding.AwayFromZero);
    }
}
