using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Publishers
{
    internal class TopicPublisher : IPublisher
    {
        private Dictionary<string, List<IOrderHandler>> _subscribers;
        private readonly object _lock = new object();

        public TopicPublisher()
        {
            _subscribers = new Dictionary<string, List<IOrderHandler>>();
        }

        public void Publish(string topic, RestaurantDocument order)
        {
            if (_subscribers.ContainsKey(topic))
            {
                foreach (var orderHandler in _subscribers[topic])
                {
                    orderHandler.Handle(order);
                }
            }
        }

        public void Subscribe(string topic, IOrderHandler handler)
        {
            var subscribers = new Dictionary<string, List<IOrderHandler>>(
                _subscribers);

            if (subscribers.ContainsKey(topic))
            {
                subscribers[topic].Add(handler);
            }
            else
            {
                subscribers.Add(topic, new List<IOrderHandler>() { handler });
            }

            lock(_lock)
            {
                _subscribers = subscribers;
            }
        }
    }
}
