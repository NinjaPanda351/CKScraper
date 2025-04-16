namespace MtgPriceUpdater.Models
{
    /// <summary>
    /// Represents a Magic: The Gathering set item that can be selected by the user.
    /// </summary>
    public class SetItem
    {
        /// <summary>
        /// Gets or sets the readable name of the MTG set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the set is selected for update.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetItem"/> class with the specified name.
        /// </summary>
        /// <param name="name">The readable name of the set.</param>
        public SetItem(string name)
        {
            Name = name;
            IsSelected = false;

            // Optional logging
            // Logger.Log($"[SetItem] Created set: {name}");
        }
    }
}