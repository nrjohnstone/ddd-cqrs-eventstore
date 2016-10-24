namespace Restaurant.Host
{
    internal class Waiter
    {
        private readonly PrinterHandler _printerHandler;

        public Waiter(PrinterHandler printerHandler)
        {
            _printerHandler = printerHandler;
        }

        public void PlaceOrder(Order order)
        {
            _printerHandler.Handle(order);
        }
    }
}