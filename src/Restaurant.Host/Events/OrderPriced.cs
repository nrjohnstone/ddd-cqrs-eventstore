using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    internal class OrderPriced : MessageBase
    {
        public RestaurantDocument Order { get;  }

        public OrderPriced(RestaurantDocument order, string messageId, string correlationId, string causativeId) 
            : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}