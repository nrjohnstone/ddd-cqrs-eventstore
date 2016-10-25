using System;
using System.Collections.Generic;
using System.Threading;

namespace Restaurant.Host
{
    internal class QueueMonitor : IStartable
    {
        private readonly IEnumerable<QueueThreadHandler> _queueThreadHandlers;
        private Thread _thread;

        public QueueMonitor(IEnumerable<QueueThreadHandler> queueThreadHandlers)
        {
            _queueThreadHandlers = queueThreadHandlers;
        }


        public void Start()
        {
            _thread = new Thread(() =>
            {
                while (true)
                {
                    foreach (var queueThreadHandler in _queueThreadHandlers)
                    {
                        Console.WriteLine($"QueueLength for {queueThreadHandler.Name} = " +
                                          $"{queueThreadHandler.Count}");
                    }
                    Thread.Sleep(1000);
                }
            });
            _thread.Start();
        }
    }
}