using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Host.Actors;
using Restaurant.Host.Dispatchers;
using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    class Program
    {
        private static Random _random;
        private static List<IStartable> _startables;
        private static TimeToLiveHandler _dispatcherTtl;
        private static TimeToLiveHandler _cookBob;
        private static TimeToLiveHandler _cookPaco;
        private static TimeToLiveHandler _cookSteve;

        static void Main(string[] args)
        {
            _random = new Random();
            _startables = new List<IStartable>();

            IOrderHandler printerHandler = new PrinterHandler();

            var cashier = new Cashier(printerHandler);

            var cashierThread = CreateQueueThreadHandler(cashier);

            IOrderHandler asstManager = new AssistantManager(cashierThread);

            var cookHandlers = CreateCooks(asstManager);

            IOrderHandler dispatcher = new BalancedRoundRobin(cookHandlers);
            var dispatcherQueueThread = new QueueThreadHandler(dispatcher);
            _startables.Add(dispatcherQueueThread);

           _dispatcherTtl = new TimeToLiveHandler(dispatcherQueueThread);
            var waiter = new Waiter(_dispatcherTtl);

            List<QueueThreadHandler> queuesToMonitor = new List<QueueThreadHandler>();
            queuesToMonitor.AddRange(cookHandlers);
            queuesToMonitor.Add(dispatcherQueueThread);
            var queueMonitor = new QueueMonitor(queuesToMonitor);
            _startables.Add(queueMonitor);
            
            _startables.ForEach(x => x.Start());

            bool shutdown = false;
            int ordersCreated = 0;

            while (!shutdown)
            {
                if (ordersCreated < 100)
                {
                    PlaceOrders(waiter, 5);
                    ordersCreated += 5;
                }
                
                var outstandingOrders = cashier.GetOutstandingOrders();

                PrintOutstandingOrders(outstandingOrders);

                foreach (var restaurantDocument in outstandingOrders)
                {
                    cashier.Pay(restaurantDocument.Id);
                }
                
                var outstandingOrdersAfterPay = cashier.GetOutstandingOrders();
                PrintOutstandingOrders(outstandingOrdersAfterPay);
                PrintDroppedMessages();
                Thread.Sleep(100);
            }
        }

        private static void PrintDroppedMessages()
        {
            var totalOrdersDropped = 
                _dispatcherTtl.TotalOrdersDropped +
                _cookBob.TotalOrdersDropped +
                _cookPaco.TotalOrdersDropped +
                _cookSteve.TotalOrdersDropped;
            Console.WriteLine($"Total dropped messages {totalOrdersDropped}");
            Console.WriteLine($"Bob dropped messages {_cookBob.TotalOrdersDropped}");
            Console.WriteLine($"Paco dropped messages {_cookPaco.TotalOrdersDropped}");
            Console.WriteLine($"Steve dropped messages {_cookSteve.TotalOrdersDropped}");
        }

        private static QueueThreadHandler[] CreateCooks(IOrderHandler asstManager)
        {
            _cookBob = new TimeToLiveHandler(
                new Cook("Bob", asstManager,
                10));

            _cookPaco = new TimeToLiveHandler(
                new Cook("Paco", asstManager,
                499));

            _cookSteve = new TimeToLiveHandler(
                new Cook("Steve", asstManager,
                1500));

            var queueThreadHandlers = new[]
            {
                CreateQueueThreadHandler(_cookBob),
                CreateQueueThreadHandler(_cookPaco),
                CreateQueueThreadHandler(_cookSteve)
            };
            return queueThreadHandlers;
        }

        private static QueueThreadHandler CreateQueueThreadHandler(IOrderHandler cashier)
        {
            var cashierThread = new QueueThreadHandler(cashier);
            _startables.Add(cashierThread);
            return cashierThread;
        }

        private static void PlaceOrders(Waiter waiter, int numberOfOrders)
        {
            for (int j = 0; j < numberOfOrders; j++)
            {
                waiter.PlaceOrder(OrderFactory.FishAndChips());
            }
        }

        private static int GetRandomCookingTime()
        {
            return _random.Next(10, 500);
        }

        private static void PrintOutstandingOrders(IEnumerable<RestaurantDocument> outstandingOrders)
        {
            foreach (RestaurantDocument order in outstandingOrders)
            {
                Console.WriteLine($"Outstanding order {order.Id}");
            }
        }
    }
}
