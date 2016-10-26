using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    public class AssistantManager : IOrderHandler<PriceOrder>
    {
        private readonly IPublisher _publisher;
        private Dictionary<string, bool> _messagesProcessed;

        public AssistantManager(IPublisher publisher)
        {
            _publisher = publisher;
            _messagesProcessed = new Dictionary<string, bool>();
        }

        public void Handle(PriceOrder message)
        {
            if (MessageAlreadyProcessed(message.MessageId))
            {
                _publisher.Publish(new DuplicateMessageDetected(
                    nameof(PriceOrder), message.MessageId,
                    message.CorrelationId, message.CausativeId));
                return;
            }
                
            RestaurantDocument order = message.Order;
            order.Items.ForEach(x => x.Price = 100);
            order.Tax = 3.5;
            order.Total = order.Items.Sum(x => x.Price);

            Thread.Sleep(100);
            string correlationId = message.CorrelationId;
            string causativeId = message.CausativeId;
            var orderPriced = new OrderPriced(order, Guid.NewGuid().ToString(), 
                correlationId, causativeId);

            MarkMessageAsProcessed(message);

            _publisher.Publish(orderPriced);
        }

        private void MarkMessageAsProcessed(PriceOrder message)
        {
            _messagesProcessed[message.MessageId] = true;
        }

        private bool MessageAlreadyProcessed(string messageId) => 
            _messagesProcessed.ContainsKey(messageId);
    }
}