using System;
using System.Collections.Generic;
using System.Threading;
using Restaurant.Host.Actors;
using Restaurant.Host.Dispatchers;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host
{
    class Program
    {
        private static Random _random;
        private static List<IStartable> _startables;
        private static TimeToLiveHandler<OrderPlaced> _cookBob;
        private static TimeToLiveHandler<OrderPlaced> _cookPaco;
        private static TimeToLiveHandler<OrderPlaced> _cookSteve;
        private static TopicPublisher _publisher;

        static void Main(string[] args)
        {
            _random = new Random();
            _startables = new List<IStartable>();
            _publisher = new TopicPublisher();

            var printerHandler = new PrinterHandler();

            var cashier = new Cashier(_publisher);

            var cashierQueue = CreateQueueThreadHandler(cashier);

            var asstManager = new AssistantManager(_publisher);

            var cookHandlers = CreateCooks();

            var dispatcher = new BalancedRoundRobin<OrderPlaced>(cookHandlers);
            var cooksDispatcherQueue = new QueueThreadedHandler<OrderPlaced>(dispatcher);
            
            _startables.Add(cooksDispatcherQueue);

            var waiter = new Waiter(_publisher);

            var queuesToMonitor = new List<IMonitorableQueue>();
            queuesToMonitor.AddRange(cookHandlers);
            queuesToMonitor.Add(cooksDispatcherQueue);
            var queueMonitor = new QueueMonitor(queuesToMonitor);
            _startables.Add(queueMonitor);

            // Subscriptions
            _publisher.Subscribe(cooksDispatcherQueue);
            _publisher.Subscribe(asstManager);
            _publisher.Subscribe(cashierQueue);
            _publisher.Subscribe(printerHandler);

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
                _cookBob.TotalOrdersDropped +
                _cookPaco.TotalOrdersDropped +
                _cookSteve.TotalOrdersDropped;
            Console.WriteLine($"Total dropped messages {totalOrdersDropped}");
            Console.WriteLine($"Bob dropped messages {_cookBob.TotalOrdersDropped}");
            Console.WriteLine($"Paco dropped messages {_cookPaco.TotalOrdersDropped}");
            Console.WriteLine($"Steve dropped messages {_cookSteve.TotalOrdersDropped}");
        }

        private static QueueThreadedHandler<OrderPlaced>[] CreateCooks()
        {
            _cookBob = new TimeToLiveHandler<OrderPlaced>(
                new Cook("Bob",
                10, _publisher));

            _cookPaco = new TimeToLiveHandler<OrderPlaced>(
                new Cook("Paco",
                499, _publisher));

            _cookSteve = new TimeToLiveHandler<OrderPlaced>(
                new Cook("Steve",
                1500, _publisher));

            var queueThreadHandlers = new[]
            {
                CreateQueueThreadHandler(_cookBob),
                CreateQueueThreadHandler(_cookPaco),
                CreateQueueThreadHandler(_cookSteve)
            };
            return queueThreadHandlers;
        }

        private static QueueThreadedHandler<T> CreateQueueThreadHandler<T>(IOrderHandler<T> cashier) where T : MessageBase
        {
            var cashierThread = new QueueThreadedHandler<T>(cashier);
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
