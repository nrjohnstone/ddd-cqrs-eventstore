using System;
using System.Collections;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class PrinterHandler : IOrderHandler
    {
        public void Handle(RestaurantDocument order)
        {
            foreach (var item in order.Items)
            {
                Console.WriteLine($"{item.Description}  ${item.Price}");
            }
            foreach (var ingredient in order.Ingredients)
            {
                Console.WriteLine($"Ingredients: {ingredient}");
            }

            Console.WriteLine($"Tax: {order.Tax}");
            Console.WriteLine($"Total: {order.Total}");
        }
    }
}