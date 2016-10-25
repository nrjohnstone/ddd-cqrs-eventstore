using Restaurant.Host.Documents;

namespace Restaurant.Host.Actors
{
    internal class OrderCooked : MessageBase
    {
        public RestaurantDocument Order { get; }

        public OrderCooked(RestaurantDocument order)
        {
            Order = order;
            Id = order.Id;
        }
    }
}