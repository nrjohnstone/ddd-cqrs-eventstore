using Restaurant.Tests;

namespace Restaurant.Host.Dispatchers
{
    internal class Fanout : IOrderHandler
    {
        private readonly IOrderHandler[] _orderHandlers;

        public Fanout(IOrderHandler[] orderHandlers)
        {
            _orderHandlers = orderHandlers;
        }

        public void Handle(RestaurantDocument order)
        {
            foreach (var orderHandler in _orderHandlers)
            {
                orderHandler.Handle(order);
            }
        }
    }
}