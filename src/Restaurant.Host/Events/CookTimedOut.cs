using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    public class CookTimedOut : MessageBase
    {
        public RestaurantDocument Order { get; set; }

        public CookTimedOut(string messageId, string correlationId, string causativeId,
            RestaurantDocument order) : 
            base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}