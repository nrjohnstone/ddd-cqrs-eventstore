using Restaurant.Host.Documents;

namespace Restaurant.Host.Actors
{
    internal class OrderPriced : MessageBase
    {
        public RestaurantDocument Order { get;  }

        public OrderPriced(RestaurantDocument order)
        {
            Order = order;
            Id = order.Id;
        }
    }
}