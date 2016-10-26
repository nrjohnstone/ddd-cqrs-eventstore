using System;
using System.Collections;
using Restaurant.Host.Actors;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;

namespace Restaurant.Host
{
    internal class PrinterHandler : IOrderHandler<OrderSpiked>
    {
        public void Handle(OrderSpiked message)
        {
            RestaurantDocument order = message.Order;

            foreach (var item in order.Items)
            {
                Console.WriteLine($"{item.Description}  ${item.Price}");
            }
            try
            {
                foreach (var ingredient in order.Ingredients)
                {
                    Console.WriteLine($"Ingredients: {ingredient}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"Tax: {order.Tax}");
            Console.WriteLine($"Total: {order.Total}");
        }
    }
}