using System;
using System.Threading;
using Restaurant.Host.Documents;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Cook : IOrderHandler
    {
        public string Name { get; }

        private readonly int _cookingTime;
        private readonly IPublisher _publisher;

        public Cook(string name, int cookingTime, IPublisher publisher)
        {
            Name = name;
            Console.WriteLine($"{Name} {_cookingTime}");
            _cookingTime = cookingTime;
            _publisher = publisher;
        }

        public void Handle(RestaurantDocument order)
        {
            Console.WriteLine($"{Name} received order");
            order.Ingredients.Add("Tomato");
            order.TimeToCookMs = 500;

            WaitForMealToBeCooked();
            MealsCooked++;
            _publisher.Publish("MealCooked", order);
        }

        public int MealsCooked { get; private set; }

        private void WaitForMealToBeCooked()
        {
            Thread.Sleep(_cookingTime);
        }
    }
}