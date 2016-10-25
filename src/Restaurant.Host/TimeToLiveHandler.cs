using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal class TimeToLiveHandler : IOrderHandler
    {
        private readonly IOrderHandler _handler;
        public int TotalOrdersDropped { get; private set; }

        public TimeToLiveHandler(IOrderHandler handler)
        {
            _handler = handler;
        }

        public void Handle(RestaurantDocument order)
        {
            if (OrderIsStale(order))
            {
                Console.WriteLine($"Dropping order {order.Id} due to staleness");
                TotalOrdersDropped++;
                return;
            }

            _handler.Handle(order);
        }

        private bool OrderIsStale(RestaurantDocument order)
        {
            return DateTime.Now > order.TimeToLive;
        }
    }
}