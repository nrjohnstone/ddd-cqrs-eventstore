using System;
using NSubstitute;
using Restaurant.Host;
using Restaurant.Host.Actors;
using Restaurant.Host.Commands;
using Restaurant.Host.Documents;
using Restaurant.Host.Events;
using Restaurant.Host.Publishers;
using Xunit;

namespace Restaurant.Tests
{
    public class MidgetTests
    {
        private IPublisher _mockPublisher;

        public MidgetTests()
        {
            _mockPublisher = NSubstitute.Substitute.For<IPublisher>();
        }

        [Fact]
        public void OrderPlaced_MidgetShouldSend_CookFood()
        {
            Midget sut = CreateSut();

            sut.Handle(new OrderPlaced(
                CreateValidRestaurantOrder(),
                DateTime.Now, "", "", ""));

            _mockPublisher.Received().Publish(Arg.Any<CookFood>());
        }

        [Fact]
        public void WhenMidgetReceived_MultipleOrderPlaced_ShouldOnlySendSingle_CookFood()
        {
            var sut = CreateSut();

            string messageId = "-1";
            string correlationId = Guid.NewGuid().ToString();

            sut.Handle(new OrderPlaced(
                CreateValidRestaurantOrder(),
                DateTime.Now, messageId, correlationId, ""));
            _mockPublisher.ClearReceivedCalls();

            // act
            sut.Handle(new OrderPlaced(
                CreateValidRestaurantOrder(),
                DateTime.Now, "", "", ""));

            _mockPublisher.DidNotReceive().Publish(Arg.Any<CookFood>());
        }

        [Fact]
        public void WhenMidgetReceived_MultipleOrderPlaced_ShouldSend_DuplicateMessageDetected()
        {
            var sut = CreateSut();

            string messageId = "-1";
            string correlationId = Guid.NewGuid().ToString();

            sut.Handle(new OrderPlaced(
                CreateValidRestaurantOrder(),
                DateTime.Now, messageId, correlationId, ""));

            sut.Handle(new OrderPlaced(
                CreateValidRestaurantOrder(),
                DateTime.Now, "", "", ""));

            _mockPublisher.Received(1).Publish(Arg.Any<DuplicateMessageDetected>());
        }

        private static RestaurantDocument CreateValidRestaurantOrder()
        {
            return new RestaurantDocument("", 10);
        }

        private Midget CreateSut()
        {
            return new Midget(_mockPublisher);
        }

        [Fact]
        public void FoodCooked_MidgetShouldSend_PriceOrder()
        {
            Midget sut = CreateSut();

            sut.Handle(new FoodCooked(CreateValidRestaurantOrder(),
                 "", "", ""));

            _mockPublisher.Received().Publish(Arg.Any<PriceOrder>());
        }

        [Fact]
        public void OrderPriced_MidgetShouldSend_TakePayment()
        {
            Midget sut = CreateSut();

            sut.Handle(new OrderPriced(CreateValidRestaurantOrder(),
                 "", "", ""));

            _mockPublisher.Received().Publish(Arg.Any<TakePayment>());
        }
    }
}