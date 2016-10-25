using System;
using System.Collections.Generic;
using System.Threading;
using Restaurant.Host.Actors;
using Restaurant.Host.Dispatchers;
using Restaurant.Host.Documents;
using Restaurant.Host.Publishers;

namespace Restaurant.Host
{
    class Program
    {
        private static Random _random;
        private static List<IStartable> _startables;
        private static TimeToLiveHandler _cookBob;
        private static TimeToLiveHandler _cookPaco;
        private static TimeToLiveHandler _cookSteve;
        private static TopicPublisher _publisher;

        static void Main(string[] args)
        {
            _random = new Random();
            _startables = new List<IStartable>();
            _publisher = new TopicPublisher();

            IOrderHandler printerHandler = new PrinterHandler();

            var cashier = new Cashier(_publisher);

            var cashierQueue = CreateQueueThreadHandler(cashier);

            IOrderHandler asstManager = new AssistantManager(_publisher);

            var cookHandlers = CreateCooks(asstManager);

            IOrderHandler dispatcher = new BalancedRoundRobin(cookHandlers);
            var cooksDispatcherQueue = new QueueThreadHandler(dispatcher);
            
            _startables.Add(cooksDispatcherQueue);

            var waiter = new Waiter(_publisher);

            List<QueueThreadHandler> queuesToMonitor = new List<QueueThreadHandler>();
            queuesToMonitor.AddRange(cookHandlers);
            queuesToMonitor.Add(cooksDispatcherQueue);
            var queueMonitor = new QueueMonitor(queuesToMonitor);
            _startables.Add(queueMonitor);

            // Subscriptions
            _publisher.Subscribe(Events.OrderCreated, cooksDispatcherQueue);
            _publisher.Subscribe("MealCooked", asstManager);
            _publisher.Subscribe("PricesAdded", cashierQueue);
            _publisher.Subscribe("OrderSpiked", printerHandler);

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

        private static QueueThreadHandler[] CreateCooks(IOrderHandler asstManager)
        {
            _cookBob = new TimeToLiveHandler(
                new Cook("Bob",
                10, _publisher));

            _cookPaco = new TimeToLiveHandler(
                new Cook("Paco",
                499, _publisher));

            _cookSteve = new TimeToLiveHandler(
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

    public static class Events
    {
        public const string OrderCreated = nameof(Events.OrderCreated);
    }
}
