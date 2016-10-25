using System.Linq;
using System.Threading;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Actors
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

            Thread.Sleep(500);
            _nextHandler.Handle(order);
        }
    }
}