using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Events
{
    internal class OrderPlaced : MessageBase, ITimeToLive
    {
        public RestaurantDocument Order { get; }

        public OrderPlaced(RestaurantDocument order, DateTime timeToLive)
        {
            Order = order;
            TimeToLive = timeToLive;
            Id = order.Id;
        }

        public DateTime TimeToLive { get; }
    }
}