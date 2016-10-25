namespace Restaurant.Host.Actors
{
    internal class Waiter
    {
        private readonly IOrderHandler _nextHandler;

        public Waiter(IOrderHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public void PlaceOrder(RestaurantDocument order)
        {
            _nextHandler.Handle(order);
        }
    }
}