using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Dispatchers
{
    internal class BalancedRoundRobin<T> : IOrderHandler<T> where T : MessageBase
    {
        private readonly IEnumerable<MonitorableQueueThreadHandler<T>> _queueThreadHandlers;

        public BalancedRoundRobin(IEnumerable<MonitorableQueueThreadHandler<T>> queueThreadHandlers)
        {
            _queueThreadHandlers = queueThreadHandlers;
        }

        private IOrderHandler<T> GetNextAvailableHandler()
        {
            foreach (var queueThreadHandler in _queueThreadHandlers)
            {
                if (queueThreadHandler.Count < 5)
                    return queueThreadHandler;
            }

            return null;
        }

        public void Handle(T message)
        {
            while (true)
            {
                IOrderHandler<T> nextValidHander =
                    GetNextAvailableHandler();

                if (nextValidHander != null)
                {
                    nextValidHander.Handle(message);
                    break;
                }

                Thread.Sleep(1);
            }
        }
    }
}