using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Publishers
{
    internal interface IPublisher
    {
        void Publish(string topic, RestaurantDocument order);
        void Publish<T>(T message);
        void Subscribe(string topic, IOrderHandler handler);
    }
}