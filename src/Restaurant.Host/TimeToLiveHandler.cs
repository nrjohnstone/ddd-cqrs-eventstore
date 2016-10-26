using System;
using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal class TimeToLiveHandler<T> : IOrderHandler<T> where T: MessageBase, ITimeToLive
    {
        private readonly IOrderHandler<T> _handler;
        public int TotalOrdersDropped { get; private set; }

        public TimeToLiveHandler(IOrderHandler<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T message)
        {
            if (DateTime.Now > message.TimeToLive)
            {
                Console.WriteLine($"Dropping order {message.MessageId} due to staleness");
                TotalOrdersDropped++;
                return;
            }

            _handler.Handle(message);
        }
    }
}