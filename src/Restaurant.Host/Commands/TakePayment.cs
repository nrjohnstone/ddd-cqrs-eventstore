using Restaurant.Host.Documents;

namespace Restaurant.Host.Commands
{
    public class TakePayment : MessageBase
    {
        public RestaurantDocument Order { get; }

        public TakePayment(RestaurantDocument order, string messageId, string correlationId, string causativeId)
            : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }
    }
}