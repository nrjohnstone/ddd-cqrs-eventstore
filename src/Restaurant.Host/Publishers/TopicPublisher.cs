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
        private Dictionary<string, List<object>> _subscribers;

        private readonly object _lock = new object();

        public TopicPublisher()
        {
            _subscribers = new Dictionary<string, List<object>>();
        }

        public void Publish<T>(string topic, T order)
        {
            if (_subscribers.ContainsKey(topic))
            {
                foreach (dynamic orderHandler in _subscribers[topic])
                {
                    orderHandler.Handle(order);
                }
            }
        }

        public void Publish<T>(T message) where T : MessageBase
        {
            string topic = typeof(T).Name;
           
            Publish(topic, message);
            Publish(message.CorrelationId, message);
        }

        public void Subscribe<T>(IOrderHandler<T> handler) where T : MessageBase
        {
            string topic = typeof(T).Name;

            Subscribe(topic, handler);
        }

        void IPublisher.Subscribe<T>(string correlationId, IOrderHandler<T> handler)
        {
            Subscribe(correlationId, handler);
        }

        void IPublisher.Unsubscribe<T>(string topic, IOrderHandler<T> handler)
        {
            if (_subscribers.ContainsKey(topic))
            {
                _subscribers.Remove(topic);
            }
        }

        protected void Subscribe<T>(string topic, IOrderHandler<T> handler)
        {
            var subscribers = new Dictionary<string, List<object>>(_subscribers);

            if (subscribers.ContainsKey(topic))
            {
                subscribers[topic].Add(handler);
            }
            else
            {
                subscribers.Add(topic, new List<object>() { handler });
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
