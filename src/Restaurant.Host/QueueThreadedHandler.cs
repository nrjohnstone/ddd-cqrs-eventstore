using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Restaurant.Host
{
    internal class QueueThreadedHandler<T> : 
        IOrderHandler<T>, IStartable, IMonitorableQueue where T:MessageBase
    {
        private readonly IOrderHandler<T> _orderHandler;
        private readonly ConcurrentQueue<T> _orderQueue;
        private Thread _thread;

        public QueueThreadedHandler(IOrderHandler<T> orderHandler)
        {
            _orderHandler = orderHandler;
            _orderQueue = new ConcurrentQueue<T>();
        }

        public void Handle(T order)
        {
            _orderQueue.Enqueue(order);
        }

        public int Count => _orderQueue.Count;
        public string Name { get; set; }

        public void Start()
        {
            _thread = new Thread(() =>
            {
                while (true)
                {
                    T order;
                    _orderQueue.TryDequeue(out order);
                    if (order != null)
                    {
                        Console.WriteLine($"Handling order {order.MessageId}");
                        _orderHandler.Handle(order);
                    }
                }
            });
            _thread.Name = $"{Name}_Thread_{_thread.ManagedThreadId}";
            _thread.Start();
        }

    }
}