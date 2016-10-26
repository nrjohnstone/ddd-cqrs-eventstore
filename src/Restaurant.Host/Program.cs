using System;
using System.Collections.Generic;
using System.Threading;
using Restaurant.Host.Actors;
using Restaurant.Host.Commands;
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
        private static TimeToLiveHandler<CookFood> _ttlBob;
        private static TimeToLiveHandler<CookFood> _ttlPaco;
        private static TimeToLiveHandler<CookFood> _ttlSteve;
        private static TopicPublisher _publisher;
        private static PrinterHandler _printerHandler;
        private static Waiter _waiter;
        private static Cashier _cashier;
        private static AssistantManager _asstManager;
        private static Cook _cookBob;
        private static Cook _cookPaco;
        private static Cook _cookSteve;

        static void Main(string[] args)
        {
            _random = new Random();
            _startables = new List<IStartable>();
            _publisher = new TopicPublisher();

            #region "Create Actors"

            _printerHandler = new PrinterHandler();
            _waiter = new Waiter(_publisher);
            _cashier = new Cashier(_publisher);
            _asstManager = new AssistantManager(_publisher);

            _cookBob = new Cook("Bob", 10, _publisher);
            _cookPaco = new Cook("Paco", 25, _publisher);
            _cookSteve = new Cook("Steve", 5000, _publisher);

            #endregion

            #region "Create TTL Handlers"

            _ttlBob = new TimeToLiveHandler<CookFood>(_cookBob);
            _ttlPaco = new TimeToLiveHandler<CookFood>(_cookPaco);
            _ttlSteve = new TimeToLiveHandler<CookFood>(_cookSteve);

            #endregion

            #region "Create Queues"

            var cashierQueue = new QueueThreadedHandler<TakePayment>(_cashier);
            var cookBobQueue = new QueueThreadedHandler<CookFood>(_ttlBob);
            var cookPacoQueue = new QueueThreadedHandler<CookFood>(_ttlPaco);
            var cookSteveQueue = new QueueThreadedHandler<CookFood>(_ttlSteve);

            var cooksReceieveQueue = new BalancedRoundRobin<CookFood>(new[]
            {
                cookBobQueue,
                cookPacoQueue,
                cookSteveQueue
            });

            var kitchenReceieveQueue = new QueueThreadedHandler<CookFood>(cooksReceieveQueue);

            #endregion

            #region "Queue Monitoring"

            var cookQueues = new[]
            {
                cookBobQueue,
                cookPacoQueue,
                cookSteveQueue
            };

            var queuesToMonitor = new List<IMonitorableQueue>();
            queuesToMonitor.AddRange(cookQueues);
            queuesToMonitor.Add(kitchenReceieveQueue);
            var queueMonitor = new QueueMonitor(queuesToMonitor);

            #endregion

            #region "Subscribe to Events"

            _publisher.Subscribe(kitchenReceieveQueue);
            _publisher.Subscribe(_asstManager);
            _publisher.Subscribe(cashierQueue);
            _publisher.Subscribe(_printerHandler);

            #endregion
              
            #region "Start threads"

            _startables.Add(cashierQueue);
            _startables.Add(cookBobQueue);
            _startables.Add(cookPacoQueue);
            _startables.Add(cookSteveQueue);
            _startables.Add(kitchenReceieveQueue);
            _startables.Add(queueMonitor);

            _startables.ForEach(x => x.Start());

            #endregion

            bool shutdown = false;
            int ordersCreated = 0;

            while (!shutdown)
            {
                if (ordersCreated < 100)
                {
                    PlaceOrders(_waiter, 5);
                    ordersCreated += 5;
                }
                
                var outstandingOrders = _cashier.GetOutstandingOrders();

                PrintOutstandingOrders(outstandingOrders);

                foreach (var restaurantDocument in outstandingOrders)
                {
                    _cashier.Pay(restaurantDocument.Id);
                }
                
                var outstandingOrdersAfterPay = _cashier.GetOutstandingOrders();
                PrintOutstandingOrders(outstandingOrdersAfterPay);
                PrintDroppedMessages();
                Thread.Sleep(500);
            }
        }

        private static void PrintDroppedMessages()
        {
            var totalOrdersDropped = 
                _ttlBob.TotalOrdersDropped +
                _ttlPaco.TotalOrdersDropped +
                _ttlSteve.TotalOrdersDropped;
            Console.WriteLine($"Total dropped messages {totalOrdersDropped}");
            Console.WriteLine($"Bob dropped messages {_ttlBob.TotalOrdersDropped}");
            Console.WriteLine($"Paco dropped messages {_ttlPaco.TotalOrdersDropped}");
            Console.WriteLine($"Steve dropped messages {_ttlSteve.TotalOrdersDropped}");
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
