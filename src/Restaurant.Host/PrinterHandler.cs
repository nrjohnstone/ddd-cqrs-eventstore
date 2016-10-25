using System;
using System.Collections;

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
            try
            {
                foreach (var ingredient in order.Ingredients)
                {
                    Console.WriteLine($"Ingredients: {ingredient}");
                }
            }
            catch (Exception ex)
            {
                
            }
            

            Console.WriteLine($"Tax: {order.Tax}");
            Console.WriteLine($"Total: {order.Total}");
        }
    }
}