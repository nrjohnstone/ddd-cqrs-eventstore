using System;

namespace Restaurant.Host
{
    internal class PrinterHandler
    {
        public void Handle(Order order)
        {
            Console.WriteLine(order.Description);
        }
    }
}