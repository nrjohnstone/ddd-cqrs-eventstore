using System;
using System.Collections.Concurrent;
using System.Threading;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class QueueThreadHandler : IOrderHandler, IStartable
    {
        private readonly IOrderHandler _orderHandler;
        private readonly ConcurrentQueue<RestaurantDocument> _orderQueue;
        private Thread _thread;

        public QueueThreadHandler(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
            _orderQueue = new ConcurrentQueue<RestaurantDocument>();
        }

        public void Handle(RestaurantDocument order)
        {
            _orderQueue.Enqueue(order);
        }

        public int Count => _orderQueue.Count;
        public string Name => _thread.Name;

        public void Start()
        {
            _thread = new Thread(() =>
            {
                while (true)
                {
                    RestaurantDocument order;
                    _orderQueue.TryDequeue(out order);
                    if (order != null)
                    {
                        Console.WriteLine($"Handling order {order.Id}");
                        _orderHandler.Handle(order);
                    }
                }
            });
            _thread.Name = $"Thread_{_thread.ManagedThreadId}";
            _thread.Start();
        }
    }
}