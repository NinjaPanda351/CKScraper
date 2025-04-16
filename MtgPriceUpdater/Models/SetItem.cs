namespace MtgPriceUpdater.Models
{
    public class SetItem
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }

        public SetItem(string name)
        {
            Name = name;
            IsSelected = false;
        }
    }
}