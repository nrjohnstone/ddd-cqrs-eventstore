using System;
using System.Threading;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class Cook : IOrderHandler<CookFood>
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

        public void Handle(CookFood message)
        {
            RestaurantDocument order = message.Order;
            Console.WriteLine($"{Name} received order");
            order.Ingredients.Add("Tomato");
            order.TimeToCookMs = 500;

            WaitForMealToBeCooked();
            MealsCooked++;
            string correlationId = message.CorrelationId;
            string causativeId = message.MessageId;
            _publisher.Publish(new FoodCooked(order, Guid.NewGuid().ToString(), correlationId,
                causativeId));
        }
    }
}