using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Host.Dispatchers;
using Restaurant.Tests;

namespace Restaurant.Host
{
    class Program
    {
        private static Random _random;
        private static List<IStartable> _startables;

        static void Main(string[] args)
        {
            _random = new Random();
            _startables = new List<IStartable>();

            IOrderHandler printerHandler = new PrinterHandler();

            var cashier = new Cashier(printerHandler);

            var cashierThread = CreateQueueThreadHandler(cashier);

            IOrderHandler asstManager = new AssistantManager(cashierThread);

            var cookHandlers = CreateCooks(asstManager);

            IOrderHandler dispatcher = new RoundRobin(cookHandlers);
            var waiter = new Waiter(dispatcher);

            var queueMonitor = new QueueMonitor(cookHandlers);
            _startables.Add(queueMonitor);
            
            _startables.ForEach(x => x.Start());

            PlaceOrders(waiter, 100);

            while (true)
            {
                var outstandingOrders = cashier.GetOutstandingOrders();

                PrintOutstandingOrders(outstandingOrders);

                foreach (var restaurantDocument in outstandingOrders)
                {
                    cashier.Pay(restaurantDocument.Id);
                }
                
                var outstandingOrdersAfterPay = cashier.GetOutstandingOrders();
                PrintOutstandingOrders(outstandingOrdersAfterPay);
            }
        }

        private static QueueThreadHandler[] CreateCooks(IOrderHandler asstManager)
        {
            var cookBob = new Cook("Bob", asstManager,
                GetRandomCookingTime());

            var cookPaco = new Cook("Paco", asstManager,
                GetRandomCookingTime());

            var cookSteve = new Cook("Steve", asstManager,
                GetRandomCookingTime());

            var queueThreadHandlers = new[]
            {
                CreateQueueThreadHandler(cookBob),
                CreateQueueThreadHandler(cookPaco),
                CreateQueueThreadHandler(cookSteve)
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
