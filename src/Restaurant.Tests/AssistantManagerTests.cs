using System;
using NSubstitute;
using Restaurant.Host.Actors;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;
using Xunit;

namespace Restaurant.Tests
{
    public class AssistantManagerTests
    {
        private IPublisher _mockPublisher;

        public AssistantManagerTests()
        {
            _mockPublisher = Substitute.For<IPublisher>();
        }

        [Fact]
        public void WhenMutliple_PriceOrderCommandsReceived_ShouldOnlySendSingle_OrderPriced()
        {
            AssistantManager sut = CreateSut();

            string messageId = Guid.NewGuid().ToString();
            string correlationId = Guid.NewGuid().ToString();

            sut.Handle(CreateValidPriceOrder(messageId, correlationId));

            _mockPublisher.ClearReceivedCalls();

            sut.Handle(CreateValidPriceOrder(messageId, correlationId));

            _mockPublisher.DidNotReceive().Publish(Arg.Any<OrderPriced>());
        }

        [Fact]
        public void WhenMutliple_PriceOrderCommandsReceived_ShouldSend_DuplicateMessage()
        {
            AssistantManager sut = CreateSut();

            string messageId = Guid.NewGuid().ToString();
            string correlationId = Guid.NewGuid().ToString();

            sut.Handle(CreateValidPriceOrder(messageId, correlationId));

            _mockPublisher.ClearReceivedCalls();

            sut.Handle(CreateValidPriceOrder(messageId, correlationId));

            _mockPublisher.Received(1).Publish(Arg.Any<DuplicateMessageDetected>());
        }

        private static PriceOrder CreateValidPriceOrder(string messageId, string correlationId)
        {
            return new PriceOrder(new RestaurantDocument("", 10),
                messageId, correlationId, "");
        }

        private AssistantManager CreateSut()
        {
            return new AssistantManager(_mockPublisher);
        }
    }
}