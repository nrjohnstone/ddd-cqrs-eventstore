namespace Restaurant.Host.Documents
{
    public class Item
    {
        public string Description { get; set; }
        public int Price { get; set; }

        public Item(string description)
        {
            Description = description;
        }
    }
}