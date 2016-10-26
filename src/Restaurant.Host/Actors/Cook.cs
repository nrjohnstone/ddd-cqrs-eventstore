using System;
using System.Threading;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Cook : IOrderHandler<OrderPlaced>
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

        public int MealsCooked { get; private set; }

        private void WaitForMealToBeCooked()
        {
            Thread.Sleep(_cookingTime);
        }

        public void Handle(OrderPlaced message)
        {
            RestaurantDocument order1 = message.Order;
            Console.WriteLine($"{Name} received order");
            order1.Ingredients.Add("Tomato");
            order1.TimeToCookMs = 500;

            WaitForMealToBeCooked();
            MealsCooked++;
            _publisher.Publish(new OrderCooked(order1));
        }
    }
}