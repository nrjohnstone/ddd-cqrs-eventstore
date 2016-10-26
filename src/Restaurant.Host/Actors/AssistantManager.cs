using System.Linq;
using System.Threading;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class AssistantManager : IOrderHandler<OrderCooked>
    {
        private readonly IPublisher _publisher;

        public AssistantManager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(RestaurantDocument order)
        {
            order.Items.ForEach(x => x.Price = 100);
            order.Tax = 3.5;
            order.Total = order.Items.Sum(x => x.Price);

            Thread.Sleep(500);
            _publisher.Publish(new OrderPriced(order));
        }

        
        public void Handle(OrderCooked message)
        {
            Handle(message.Order);
        }
    }
}