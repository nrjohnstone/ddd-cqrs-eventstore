using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Tests;

namespace Restaurant.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            IOrderHandler printerHandler = new PrinterHandler();
            var cassier = new Cassier(printerHandler);
            IOrderHandler asstManager = new AssistantManager(cassier);
            IOrderHandler cook = new Cook(asstManager);
            var waiter = new Waiter(cook);
            
            waiter.PlaceOrder(OrderFactory.FishAndChips());
            waiter.PlaceOrder(OrderFactory.Hamburger());
            waiter.PlaceOrder(OrderFactory.Haggis());
            waiter.PlaceOrder(OrderFactory.Kapsalon());

            var outstandingOrders = cassier.GetOutstandingOrders();

            PrintOutstandingOrders(outstandingOrders);
            cassier.Pay("1");
            var outstandingOrdersAfterPay = cassier.GetOutstandingOrders();
            PrintOutstandingOrders(outstandingOrdersAfterPay);
            Console.ReadLine();
        }

        private static void PrintOutstandingOrders(RestaurantDocument[] outstandingOrders)
        {
            foreach (RestaurantDocument order in outstandingOrders)
            {
                Console.WriteLine(order.Id);
            }
        }
    }

    internal class Cassier : IOrderHandler
    {
        private readonly IOrderHandler _nextHandler;
        private readonly Dictionary<string, RestaurantDocument> _unpaidOrders;

        public Cassier(IOrderHandler nextHandler)
        {
            _nextHandler = nextHandler;
            _unpaidOrders = new Dictionary<string, RestaurantDocument>();
        }


        public void Handle(RestaurantDocument order)
        {
            _unpaidOrders.Add(order.Id, order);
            _nextHandler.Handle(order);
        }

        public void Pay(string id)
        {
            _unpaidOrders.Remove(id);
        }

        public RestaurantDocument[] GetOutstandingOrders()
        {
            return _unpaidOrders.Values.ToArray();
        }
    }
}
