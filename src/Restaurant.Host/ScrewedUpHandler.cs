using System;

namespace Restaurant.Host
{
    public class ScrewedUpHandler<T> :
        IOrderHandler<T> where T : MessageBase
    {
        private readonly IOrderHandler<T> _handler;
        private readonly int _probabilityToDrop;
        private readonly int _probabilityToDuplicate;
        private Random _random;

        public ScrewedUpHandler(IOrderHandler<T> handler, int probabilityToDrop, int probabilityToDuplicate)
        {
            _handler = handler;
            _probabilityToDrop = probabilityToDrop;
            _probabilityToDuplicate = probabilityToDuplicate;

            if (_probabilityToDrop + _probabilityToDuplicate > 100)
                throw new Exception("WTF");

            _random = new Random();
        }

        public void Handle(T message)
        {
            var chance = _random.Next(1, 100);
            int dropThreshold = 0 + _probabilityToDrop;
            int duplicateThreshold = dropThreshold + _probabilityToDuplicate;

            if (chance < dropThreshold)
            {
                return;
            }

            if (chance < duplicateThreshold)
            {
                _handler.Handle(message);
                _handler.Handle(message);
                return;
            }

            _handler.Handle(message);
        }
    }
}