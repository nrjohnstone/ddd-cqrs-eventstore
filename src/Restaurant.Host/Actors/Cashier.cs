using System;
using System.Collections.Concurrent;
using System.Linq;
using Restaurant.Host.Documents;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Cashier : IOrderHandler
    {
        private readonly IPublisher _publisher;
        private readonly ConcurrentDictionary<string, RestaurantDocument> _unpaidOrders;

        public Cashier(IPublisher publisher)
        {
            _publisher = publisher;
            _unpaidOrders = new ConcurrentDictionary<string, RestaurantDocument>();
        }

        public void Handle(RestaurantDocument order)
        {
            bool orderHandler = false;
            while (!orderHandler)
            {
                orderHandler = _unpaidOrders.TryAdd(order.Id, order);
            }
            _publisher.Publish("OrderSpiked", order);
        }

        public void Pay(string id)
        {
            bool orderHandler = false;
            while (!orderHandler)
            {
                RestaurantDocument doc;
                orderHandler = _unpaidOrders.TryRemove(id, out doc);
            }

            Console.WriteLine($"Order {id} has been paid");
        }

        public RestaurantDocument[] GetOutstandingOrders()
        {
            return _unpaidOrders.Values.ToArray();
        }
    }
}