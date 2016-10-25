using Restaurant.Host.Documents;

namespace Restaurant.Host.Actors
{
    internal class OrderSpiked : MessageBase
    {
        public RestaurantDocument Order { get; }

        public OrderSpiked(RestaurantDocument order)
        {
            Order = order;
            Id = order.Id;
        }
    }
}