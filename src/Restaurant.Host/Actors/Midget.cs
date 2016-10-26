using System;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    public class Midget : IMidget
    {
        private readonly IPublisher _publisher;
        private bool _orderPlacedReceived;
        private bool _orderPriced;
        private bool _foodHasBeenCooked;

        public Midget(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(MessageBase message)
        {
            if (message is OrderPlaced)
            {
                if (!_orderPlacedReceived)
                {
                    _orderPlacedReceived = true;
                    SendCookFoodCommand(message, ((OrderPlaced) message).Order);
                }
                else
                {
                    _publisher.Publish(new DuplicateMessageDetected(
                        nameof(OrderPlaced),
                        message.MessageId, message.CorrelationId, "" ));
                }
            }

            if (message is FoodCooked)
            {
                _foodHasBeenCooked = true;
                _publisher.Publish(new PriceOrder(
                    ((FoodCooked) message).Order,
                    Guid.NewGuid().ToString(),
                    correlationId: message.CorrelationId,
                    causativeId: message.MessageId));
            }

            if (message is OrderPriced)
            {
                if (!_orderPriced)
                {
                    _orderPriced = true;
                    _publisher.Publish(new TakePayment(
                        ((OrderPriced) message).Order,
                        Guid.NewGuid().ToString(),
                        correlationId: message.CorrelationId,
                        causativeId: message.MessageId));
                }
                else
                {
                    _publisher.Publish(new DuplicateMessageDetected(
                        nameof(OrderPriced), 
                        message.MessageId, message.CorrelationId, ""));
                }
            }

            if (message is OrderSpiked)
            {
                // DIE
            }

            if (message is CookTimedOut)
            {
                if (_foodHasBeenCooked)
                    return;

                SendCookFoodCommand(message, ((CookTimedOut) message).Order);
            }
        }

        private void SendCookFoodCommand(MessageBase message, RestaurantDocument restaurantDocument)
        {
            var timeToLive = DateTime.Now + TimeSpan.FromSeconds(50);
            var messageId = Guid.NewGuid().ToString();

            _publisher.Publish(new CookFood(order: restaurantDocument,
                messageId: messageId,
                correlationId: message.CorrelationId,
                causativeId: message.MessageId,
                timeToLive: timeToLive));

            _publisher.Publish(new DelayPublish(
                Guid.NewGuid().ToString(),
                message.CorrelationId,
                message.MessageId,
                DateTime.Now + TimeSpan.FromSeconds(1),
                new CookTimedOut(
                    Guid.NewGuid().ToString(),
                    message.CorrelationId,
                    message.MessageId,
                    restaurantDocument)));
        }
    }
}