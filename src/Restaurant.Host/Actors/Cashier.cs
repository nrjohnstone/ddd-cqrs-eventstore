using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Restaurant.Host.Actors
{
    internal class Cashier : IOrderHandler
    {
        private readonly IOrderHandler _nextHandler;
        private readonly ConcurrentDictionary<string, RestaurantDocument> _unpaidOrders;

        public Cashier(IOrderHandler nextHandler)
        {
            _nextHandler = nextHandler;
            _unpaidOrders = new ConcurrentDictionary<string, RestaurantDocument>();
        }

        public void Handle(RestaurantDocument order)
        {
            bool orderHandler = false;
            while (!orderHandler)
            {
                orderHandler = _unpaidOrders.TryAdd(order.Id, order);
            }
            _nextHandler.Handle(order);
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