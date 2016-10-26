using System;
using System.Collections.Concurrent;
using System.Linq;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Cashier : IOrderHandler<TakePayment>
    {
        private readonly IPublisher _publisher;
        private readonly ConcurrentDictionary<string, RestaurantDocument> _unpaidOrders;

        public Cashier(IPublisher publisher)
        {
            _publisher = publisher;
            _unpaidOrders = new ConcurrentDictionary<string, RestaurantDocument>();
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

        public void Handle(TakePayment message)
        {
            RestaurantDocument order = message.Order;

            bool orderHandler = false;
            while (!orderHandler)
            {
                orderHandler = _unpaidOrders.TryAdd(order.Id, order);
            }
            string correlationId = message.CorrelationId;
            string causativeId = message.MessageId;
            OrdersPaid++;
            _publisher.Publish(new OrderSpiked(order, Guid.NewGuid().ToString(), correlationId,
                causativeId));
        }

        public int OrdersPaid { get; set; }
    }
}