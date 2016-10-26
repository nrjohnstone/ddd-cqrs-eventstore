using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    public class FoodCooked : MessageBase
    {
        public RestaurantDocument Order { get; }

        public FoodCooked(RestaurantDocument order, string messageId, string correlationId, string causativeId) 
            : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}