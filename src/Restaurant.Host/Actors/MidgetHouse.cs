using System;
using System.Collections.Generic;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class MidgetHouse<T> : IOrderHandler<OrderPlaced>, IOrderHandler<MessageBase>
    {
        private readonly IPublisher _publisher;
        private Dictionary<string, IMidget> _midgets;

        public QueueThreadedHandler<MessageBase> MidgetDispatchQueue { get; set; }

        public MidgetHouse(IPublisher publisher)
        {
            _publisher = publisher;
            _midgets = new Dictionary<string, IMidget>();
        }

        public void Handle(OrderPlaced message)
        {
            var midget = MidgetFactory.Create(message.Order.IsDodgy, _publisher);

            _midgets.Add(message.CorrelationId, midget);
            _publisher.Subscribe(message.CorrelationId, MidgetDispatchQueue);

            Console.WriteLine($"MidgetFactory received OrderPlaced");
        }

        public void Handle(MessageBase message)
        {
            if (_midgets.ContainsKey(message.CorrelationId))
            {
                _midgets[message.CorrelationId].Handle(message);
            }
        }
    }

    internal class MidgetFactory
    {
        public static IMidget Create(bool isDodgy, IPublisher publisher)
        {
            if (isDodgy)
                return new DodgyMidget(publisher);
            else
            {
                return new Midget(publisher);
            }
        }
    }
}