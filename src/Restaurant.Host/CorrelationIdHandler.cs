using System;

namespace Restaurant.Host
{
    internal class CorrelationIdHandler<T> : IOrderHandler<T> where T : MessageBase
    {
        public void Handle(T message)
        {
            Console.WriteLine($"Saw event {message.GetType().Name} with " +
                              $"correlationId {message.CorrelationId}");
        }
    }
}