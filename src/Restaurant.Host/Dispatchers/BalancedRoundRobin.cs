using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Restaurant.Host.Dispatchers
{
    internal class BalancedRoundRobin : IOrderHandler
    {
        private readonly IEnumerable<QueueThreadHandler> _queueThreadHandlers;

        public BalancedRoundRobin(IEnumerable<QueueThreadHandler> queueThreadHandlers)
        {
            _queueThreadHandlers = queueThreadHandlers;
        }

        public void Handle(RestaurantDocument order)
        {
            while (true)
            {
                IOrderHandler nextValidHander =
                    GetNextAvailableHandler();

                if (nextValidHander != null)
                {
                    nextValidHander.Handle(order);
                    break;
                }

                Thread.Sleep(1);
            }
        }

        private IOrderHandler GetNextAvailableHandler()
        {
            foreach (var queueThreadHandler in _queueThreadHandlers)
            {
                if (queueThreadHandler.Count < 5)
                    return queueThreadHandler;
            }

            return null;
        }
    }
}