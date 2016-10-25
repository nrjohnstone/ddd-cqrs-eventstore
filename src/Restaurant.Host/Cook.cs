using System;
using System.Threading;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class Cook : IOrderHandler
    {
        public string Name { get; }
        private readonly IOrderHandler _nextHandler;

        public Cook(string name, IOrderHandler nextHandler)
        {
            Name = name;
            _nextHandler = nextHandler;
        }

        public void Handle(RestaurantDocument order)
        {
            Console.WriteLine($"{Name} received order");
            order.Ingredients.Add("Tomato");
            order.TimeToCookMs = 500;
            Thread.Sleep(500);
            _nextHandler.Handle(order);
        }
    }
}