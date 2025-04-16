namespace MtgPriceUpdater.Models
{
    /// <summary>
    /// Represents a Magic: The Gathering card with relevant metadata for pricing and identification.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The name of the card (e.g., "Lightning Bolt").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The collector code of the card (e.g., "123" or "123F" if foil).
        /// </summary>
        public string CollectorCode { get; set; } = "Unknown";

        /// <summary>
        /// The card's rarity (C = Common, UC = Uncommon, R = Rare, M = Mythic).
        /// </summary>
        public string Rarity { get; set; } = "Unknown";

        /// <summary>
        /// The card's price as a string, formatted with two decimal places (e.g., "2.50").
        /// </summary>
        public string Price { get; set; } = "0.00";

        /// <summary>
        /// Indicates whether the card is a foil version.
        /// </summary>
        public bool IsFoil { get; set; } = false;

        /// <summary>
        /// The code of the set this card belongs to (e.g., "TDM").
        /// </summary>
        public string SetCode { get; set; }

        /// <summary>
        /// Returns the unique item code in the format "SETCODE CollectorCode[F]".
        /// </summary>
        public string ItemCode => $"{SetCode} {CollectorCode}{(IsFoil ? "F" : "")}";
    }
}