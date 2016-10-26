using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    internal class OrderSpiked : MessageBase
    {
        public RestaurantDocument Order { get; }

        public OrderSpiked(RestaurantDocument order, string messageId, string correlationId, string causativeId) 
            : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}