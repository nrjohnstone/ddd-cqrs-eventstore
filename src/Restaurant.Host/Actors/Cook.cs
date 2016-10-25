using System;
using System.Threading;
using Restaurant.Host.Documents;

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
            MealsCooked++;
            _nextHandler.Handle(order);
        }

        public int MealsCooked { get; private set; }

        private void WaitForMealToBeCooked()
        {
            Thread.Sleep(_cookingTime);
        }
    }
}