using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            PrinterHandler printerHandler = new PrinterHandler();
            var waiter = new Waiter(printerHandler);

            waiter.PlaceOrder(OrderFactory.FishAndChips());
            waiter.PlaceOrder(OrderFactory.Hamburger());
            waiter.PlaceOrder(OrderFactory.Haggis());
            waiter.PlaceOrder(OrderFactory.Kapsalon());

            Console.ReadLine();
        }
    }
}
