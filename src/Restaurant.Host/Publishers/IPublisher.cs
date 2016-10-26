using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Publishers
{
    internal interface IPublisher
    {
        void Publish<T>(T message) where T : MessageBase;
        void Subscribe<T>(IOrderHandler<T> handler) where T : MessageBase;
        void Subscribe<T>(string correlationId, IOrderHandler<T> handler) where T : MessageBase;
        void Unsubscribe<T>(string correlationId, IOrderHandler<T> handler);
    }
}