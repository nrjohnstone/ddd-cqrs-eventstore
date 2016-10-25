using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Host.Dispatchers;
using Restaurant.Tests;

namespace Restaurant.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrderHandler printerHandler = new PrinterHandler();
            var cashier = new Cashier(printerHandler);
            var cashierThread = new QueueThreadHandler(cashier);

            IOrderHandler asstManager = new AssistantManager(cashierThread);

            var cookBob = new QueueThreadHandler(
                new Cook("Bob", asstManager));

            var cookPaco = new QueueThreadHandler(
                new Cook("Paco", asstManager));

            var cookSteve = new QueueThreadHandler(
                new Cook("Steve", asstManager));

            IOrderHandler fanoutCooks = new RoundRobin(new[] {
                cookBob, cookPaco, cookSteve
            });
            var waiter = new Waiter(fanoutCooks);

            cookBob.Start();
            cookPaco.Start();
            cookSteve.Start();
            cashierThread.Start();

            waiter.PlaceOrder(OrderFactory.FishAndChips());
            waiter.PlaceOrder(OrderFactory.Hamburger());
            waiter.PlaceOrder(OrderFactory.Haggis());
            waiter.PlaceOrder(OrderFactory.Kapsalon());

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

        private static void PrintOutstandingOrders(RestaurantDocument[] outstandingOrders)
        {
            foreach (RestaurantDocument order in outstandingOrders)
            {
                Console.WriteLine($"Outstanding order {order.Id}");
            }
        }
    }
}
