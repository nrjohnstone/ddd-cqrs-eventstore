using System;
using System.Linq;
using System.Threading;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    internal class AssistantManager : IOrderHandler<PriceOrder>
    {
        private readonly IPublisher _publisher;

        public AssistantManager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(PriceOrder message)
        {
            RestaurantDocument order = message.Order;
            order.Items.ForEach(x => x.Price = 100);
            order.Tax = 3.5;
            order.Total = order.Items.Sum(x => x.Price);

            Thread.Sleep(500);
            string correlationId = message.CorrelationId;
            string causativeId = message.CausativeId;
            var orderPriced = new OrderPriced(order, Guid.NewGuid().ToString(), 
                correlationId, causativeId);
            _publisher.Publish(orderPriced);
        }
    }
    
}