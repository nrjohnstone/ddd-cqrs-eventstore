using System;
using System.Linq;
using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class AssistantManager : IOrderHandler
    {
        private readonly IOrderHandler _nextHandler;

        public AssistantManager(IOrderHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public void Handle(RestaurantDocument order)
        {
            order.Items.ForEach(x => x.Price = 100);
            order.Tax = 3.5;
            order.Total = order.Items.Sum(x => x.Price);
            _nextHandler.Handle(order);
        }
    }
}