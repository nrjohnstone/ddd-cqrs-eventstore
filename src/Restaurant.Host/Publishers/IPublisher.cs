using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Publishers
{
    internal interface IPublisher
    {
        void Publish<T>(T message);
        void Subscribe<T>(IOrderHandler<T> handler);
    }
}