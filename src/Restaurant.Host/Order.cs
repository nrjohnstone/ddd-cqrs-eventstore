namespace Restaurant.Host
{
    internal class Order
    {
        public Order(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}