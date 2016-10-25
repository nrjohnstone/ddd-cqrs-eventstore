using System.Threading;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class Cook : IOrderHandler
    {
        private readonly IOrderHandler _nextHandler;

        public Cook(IOrderHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public void Handle(RestaurantDocument order)
        {
            order.Ingredients.Add("Tomato");
            order.TimeToCookMs = 500;
            Thread.Sleep(500);
            _nextHandler.Handle(order);
        }
    }
}