using Restaurant.Host.Documents;

namespace Restaurant.Host.Commands
{
    internal class PriceOrder : MessageBase
    {
        public RestaurantDocument Order { get;  }

        public PriceOrder(RestaurantDocument order, string messageId, string correlationId, string causativeId) : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}