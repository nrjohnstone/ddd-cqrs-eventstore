using System;
using System.Threading;

namespace Restaurant.Host.Actors
{
    internal class Cook : IOrderHandler
    {
        public string Name { get; }
        private readonly IOrderHandler _nextHandler;
 
        private readonly int _cookingTime;

        public Cook(string name, IOrderHandler nextHandler, int cookingTime)
        {
            Name = name;
            Console.WriteLine($"{Name} {_cookingTime}");
            _nextHandler = nextHandler;
            _cookingTime = cookingTime;
        }

        public void Handle(RestaurantDocument order)
        {
            Console.WriteLine($"{Name} received order");
            order.Ingredients.Add("Tomato");
            order.TimeToCookMs = 500;

            WaitForMealToBeCooked();

            _nextHandler.Handle(order);
        }

        private void WaitForMealToBeCooked()
        {
            Thread.Sleep(_cookingTime);
        }
    }
}