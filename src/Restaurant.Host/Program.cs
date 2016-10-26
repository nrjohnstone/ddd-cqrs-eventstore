using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private static MidgetHouse<OrderPlaced> _midgetHouse;
        private static QueueThreadedHandler<MessageBase> _midgetDispatchQueue;
        private static ScrewedUpHandler<CookFood> _screwedBob;
        private static ScrewedUpHandler<CookFood> _screwedPaco;
        private static ScrewedUpHandler<CookFood> _screwedSteve;
        private static ScrewedUpHandler<PriceOrder> _screwedAsstManager;
        private static AlarmClock _alarmClock;

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
            _cookSteve = new Cook("Steve", 500, _publisher);

            _midgetHouse = new MidgetHouse<OrderPlaced>(_publisher);
            _midgetDispatchQueue = new QueueThreadedHandler<MessageBase>(_midgetHouse);
            _midgetDispatchQueue.Name = "MidgetDispatchQueue";
            _midgetHouse.MidgetDispatchQueue = _midgetDispatchQueue;

            _alarmClock = new AlarmClock(_publisher);

            #endregion

            #region "Create TTL Handlers"

            _ttlBob = new TimeToLiveHandler<CookFood>(_cookBob);
            _ttlPaco = new TimeToLiveHandler<CookFood>(_cookPaco);
            _ttlSteve = new TimeToLiveHandler<CookFood>(_cookSteve);

            #endregion

            #region "Created ScrewedUp Handlers"

            _screwedBob = new ScrewedUpHandler<CookFood>(_ttlBob, 50, 0);
            _screwedPaco = new ScrewedUpHandler<CookFood>(_ttlPaco, 0, 0);
            _screwedSteve = new ScrewedUpHandler<CookFood>(_ttlSteve, 0, 0);
            _screwedAsstManager = new ScrewedUpHandler<PriceOrder>(_asstManager, 0, 0);

            #endregion

            #region "Create Queues"

            var cashierQueue = new QueueThreadedHandler<TakePayment>(_cashier);
            var cookBobQueue = new QueueThreadedHandler<CookFood>(_screwedBob);
            cookBobQueue.Name = "BobsQueue";
            var cookPacoQueue = new QueueThreadedHandler<CookFood>(_screwedPaco);
            cookPacoQueue.Name = "PacosQueue";
            var cookSteveQueue = new QueueThreadedHandler<CookFood>(_screwedSteve);
            cookSteveQueue.Name = "StevesQueue";

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
            _publisher.Subscribe(_screwedAsstManager);
            _publisher.Subscribe(cashierQueue);
            _publisher.Subscribe(_printerHandler);
            _publisher.Subscribe<OrderPlaced>(_midgetHouse);
            _publisher.Subscribe(_alarmClock);

            #endregion

            #region "Start threads"

            _startables.Add(cashierQueue);
            _startables.Add(cookBobQueue);
            _startables.Add(cookPacoQueue);
            _startables.Add(cookSteveQueue);
            _startables.Add(kitchenReceieveQueue);
            _startables.Add(queueMonitor);
            _startables.Add(_midgetDispatchQueue);
            _startables.Add(_alarmClock);

            _startables.ForEach(x => x.Start());

            #endregion

            bool shutdown = false;
            int ordersCreated = 0;

            while (!shutdown)
            {
                if (ordersCreated < 10)
                {
                    PlaceOrders(_waiter, 1);
                    ordersCreated += 1;
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

                if (TotalOrdersPaid + TotalOrdersDropped >= 100 &&
                    TotalOrdersCooked + TotalOrdersDropped >= 100)
                    shutdown = true;

                Thread.Sleep(500);
            }

            queueMonitor.Stop();
            Console.WriteLine("<<FINISHED>>");
            Console.ReadLine();
        }

        private static void PrintDroppedMessages()
        {
            var totalOrdersDropped = TotalOrdersDropped;

            Console.WriteLine($"Total dropped messages {totalOrdersDropped}");
            Console.WriteLine($"Bob dropped messages {_ttlBob.TotalOrdersDropped}");
            Console.WriteLine($"Paco dropped messages {_ttlPaco.TotalOrdersDropped}");
            Console.WriteLine($"Steve dropped messages {_ttlSteve.TotalOrdersDropped}");
        }

        private static int TotalOrdersDropped
        {
            get
            {
                var totalOrdersDropped =
                    _ttlBob.TotalOrdersDropped +
                    _ttlPaco.TotalOrdersDropped +
                    _ttlSteve.TotalOrdersDropped;
                return totalOrdersDropped;
            }
        }

        private static int TotalOrdersCooked
        {
            get
            {
                var totalOrdersDropped =
                    _cookBob.MealsCooked +
                    _cookPaco.MealsCooked +
                    _cookSteve.MealsCooked;
                return totalOrdersDropped;
            }
        }

        private static int TotalOrdersPaid => _cashier.OrdersPaid;

        private static void PlaceOrders(Waiter waiter, int numberOfOrders)
        {
            for (int j = 0; j < numberOfOrders; j++)
            {
                var order = OrderFactory.FishAndChips();
                //order.IsDodgy = j % 2 == 0;
                order.IsDodgy = false;
                waiter.PlaceOrder(order);
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
