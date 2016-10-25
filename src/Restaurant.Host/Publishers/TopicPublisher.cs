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
        private Dictionary<Type, List<IOrderMessageHandler>> _subscribersByType;

        private readonly object _lock = new object();

        public TopicPublisher()
        {
            _subscribers = new Dictionary<string, List<IOrderHandler>>();
            _subscribersByType = new Dictionary<Type, List<IOrderMessageHandler>>();
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

        public void Publish<T>(T message)
        {
            if (_subscribersByType.ContainsKey(typeof(T)))
            {
                foreach (var orderHandler in _subscribersByType[typeof(T)])
                {
                    orderHandler.Handle(message);
                }
            }
        }

        public void Subscribe<T>(IOrderMessageHandler handler)
        {
            var subscribers = new Dictionary<Type, List<IOrderMessageHandler>>(
                _subscribersByType);

            var topic = typeof(T);

            if (subscribers.ContainsKey(topic))
            {
                subscribers[topic].Add(handler);
            }
            else
            {
                subscribers.Add(topic, new List<IOrderMessageHandler>() { handler });
            }

            lock (_lock)
            {
                _subscribersByType = subscribers;
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

    internal interface IOrderMessageHandler
    {
        void Handle(object message);
    }
}
