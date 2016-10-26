using System;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Waiter
    {
        private readonly IPublisher _publisher;
        bool _firstOrder = true;

        public Waiter(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void PlaceOrder(RestaurantDocument order)
        {
            string correlationId = Guid.NewGuid().ToString();
            string causativeId = "-1";

            if (_firstOrder)
            {
                _firstOrder = false;
                var watcher = new CorrelationIdHandler<MessageBase>();
                _publisher.Subscribe(correlationId, watcher);
            }
            _publisher.Publish(new OrderPlaced(order, 
                DateTime.Now + TimeSpan.FromMilliseconds(5000), Guid.NewGuid().ToString(), 
                correlationId, causativeId));
        }
    }
}