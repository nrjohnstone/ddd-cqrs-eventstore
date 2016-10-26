using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    internal class OrderPlaced : MessageBase, ITimeToLive
    {
        public RestaurantDocument Order { get; }

        public OrderPlaced(RestaurantDocument order, DateTime timeToLive, string messageId, string correlationId, string causativeId) 
            : base(messageId, correlationId, causativeId)
        {
            Order = order;
            TimeToLive = timeToLive;
        }

        public DateTime TimeToLive { get; }
    }
}