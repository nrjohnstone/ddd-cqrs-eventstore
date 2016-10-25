﻿using Restaurant.Host.Documents;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Waiter
    {
        private readonly IPublisher _publisher;

        public Waiter(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void PlaceOrder(RestaurantDocument order)
        {
            _publisher.Publish(Events.OrderCreated, order);
        }
    }
}