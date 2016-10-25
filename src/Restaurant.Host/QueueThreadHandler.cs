using System.Collections.Concurrent;
using System.Threading;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class QueueThreadHandler : IOrderHandler, IStartable
    {
        private readonly IOrderHandler _orderHandler;
        private readonly ConcurrentQueue<RestaurantDocument> _orderQueue;

        public QueueThreadHandler(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
            _orderQueue = new ConcurrentQueue<RestaurantDocument>();
        }

        public void Handle(RestaurantDocument order)
        {
            _orderQueue.Enqueue(order);
        }

        public void Start()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    RestaurantDocument order;
                    _orderQueue.TryDequeue(out order);
                    if (order != null)
                        _orderHandler.Handle(order);
                }
            });
            thread.Start();
        }
    }
}