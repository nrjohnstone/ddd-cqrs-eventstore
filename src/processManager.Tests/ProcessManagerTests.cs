using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace processManager.Tests
{
    public class ProcessManagerTests
    {
        private readonly IPublisher _mockPublisher = Substitute.For<IPublisher>();

        [Fact]
        public void Should_create_ProcessManager()
        {
            var sut = CreateSut();

            sut.Should().NotBeNull();
        }

    
        [Fact]
        public void When_PositionAcquired_ShouldPublishThresholdUpdated()
        {
            var sut = CreateSut();

            int startPrice = 100;
            int expectedThresholdPrice = 90;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            _mockPublisher.Received().Publish(Arg.Any<ThresholdUpdatedMessage>());
        }

        [Fact]
        public void When_PublishThresholdUpdated_ShouldBePriceMinus10()
        {
            var sut = CreateSut();

            int startPrice = 100;
            int expectedThresholdPrice = 90;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            _mockPublisher.Received().Publish(Arg.Is<ThresholdUpdatedMessage>(x => x.ThresholdPrice == expectedThresholdPrice));
        }

        [Fact]
        public void When_PriceUpdated_ShouldPublishSevenSecondMessage()
        {
            var sut = CreateSut();

            int expectedUpdatePrice = 15;

            sut.ProcessMessage(new PriceUpdateMessage(expectedUpdatePrice));

            _mockPublisher.Received(1).Publish(
                Arg.Is<PublishMessage>(x => x.Interval == 7 &&
                x.Price == expectedUpdatePrice));

        }

        [Fact]
        public void When_PriceUpdated_ShouldPublishElevenSecondMessage()
        {
            var sut = CreateSut();

            int expectedUpdatePrice = 15;

            sut.ProcessMessage(new PriceUpdateMessage(expectedUpdatePrice));

            _mockPublisher.Received(1).Publish(
                Arg.Is<PublishMessage>(x => x.Interval == 11 &&
                x.Price == expectedUpdatePrice));
        }

        [Fact]
        public void When_A_SevenSecondPrice_IsAboveThreshold_ShouldChangeThreshold()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(105));

            sut.ProcessMessage(new RemoveFromSevenSecondMessage(105));

            _mockPublisher.Received().Publish(Arg.Is<ThresholdUpdatedMessage>(
                x => x.ThresholdPrice == 95));
        }

        [Fact]
        public void When_A_SevenSecondPrice_IsBelowThreshold_ShouldNotChangeThreshold()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(89));
            _mockPublisher.ClearReceivedCalls();

            // act
            sut.ProcessMessage(new RemoveFromSevenSecondMessage(89));

            _mockPublisher.DidNotReceive().Publish(Arg.Any<ThresholdUpdatedMessage>());
        }

        [Fact]
        public void When_SevenSecondMessageReceievedOutOfOrder_ShouldRemoveCorrectPrice()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(110));
            sut.ProcessMessage(new PriceUpdateMessage(103));
            sut.ProcessMessage(new PriceUpdateMessage(105));

            sut.ProcessMessage(new RemoveFromSevenSecondMessage(103));

            _mockPublisher.Received().Publish(Arg.Is<ThresholdUpdatedMessage>(
                x => x.ThresholdPrice == 93));

            sut.ProcessMessage(new RemoveFromSevenSecondMessage(110));
            _mockPublisher.Received().Publish(Arg.Is<ThresholdUpdatedMessage>(
                x => x.ThresholdPrice == 95));
        }

        [Fact]
        public void When_AllMinimumSevenSecondMessage_AreBelowThreshold_ShouldNotChangeThreshold()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(99));
            sut.ProcessMessage(new PriceUpdateMessage(99));

            sut.ProcessMessage(new RemoveFromSevenSecondMessage(99));

            _mockPublisher.Received().Publish(Arg.Is<ThresholdUpdatedMessage>(
                x => x.ThresholdPrice == 90));
        }
        
        [Fact]
        public void When_AllMinimumElevenSecondMessages_AreAboveThreshold_ShouldNotPublishStopLossMessage()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(105));
            sut.ProcessMessage(new PriceUpdateMessage(110));

            sut.ProcessMessage(new RemoveFromElevenSecondMessage(105));

            _mockPublisher.Received(0).Publish(Arg.Any<StopLossHitMessage>());
        }

        [Fact]
        public void When_LatestElevenSecondMessage_IsBelowThreshold_ShouldNotPublishStopLossMessage()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(90));
            sut.ProcessMessage(new PriceUpdateMessage(89));

            sut.ProcessMessage(new RemoveFromElevenSecondMessage(90));

            _mockPublisher.Received(0).Publish(Arg.Any<StopLossHitMessage>());
        }

        [Fact]
        public void When_AllElevenSecondMessages_AreBelowThreshold_ShouldPublishStopLossMessage()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(89));
            sut.ProcessMessage(new PriceUpdateMessage(89));

            sut.ProcessMessage(new RemoveFromElevenSecondMessage(89));

            _mockPublisher.Received().Publish(Arg.Any<StopLossHitMessage>());
        }

        [Fact]
        public void Should_onlySendStopLossHitMessage_Once()
        {
            var sut = CreateSut();
            int startPrice = 100;
            sut.ProcessMessage(new PositionAcquiredMessage(startPrice));

            sut.ProcessMessage(new PriceUpdateMessage(89));
            sut.ProcessMessage(new PriceUpdateMessage(88));
            sut.ProcessMessage(new PriceUpdateMessage(87));

            sut.ProcessMessage(new RemoveFromElevenSecondMessage(89));

            _mockPublisher.Received().Publish(Arg.Any<StopLossHitMessage>());
            _mockPublisher.ClearReceivedCalls();

            sut.ProcessMessage(new RemoveFromElevenSecondMessage(88));
            _mockPublisher.DidNotReceive().Publish(Arg.Any<StopLossHitMessage>());
        }

        private ProcessManager CreateSut()
        {
            return new ProcessManager(_mockPublisher);
        }
    }

    public class PriceUpdateMessage : BaseMessage
    {
        public PriceUpdateMessage(int updatedPrice)
        {
            Value = updatedPrice;
        }
    }

    public class PositionAcquiredMessage : BaseMessage
    {
        
        public PositionAcquiredMessage(int startPosition)
        {
            Value = startPosition;
        }
    }

    public class RemoveFromSevenSecondMessage : BaseMessage
    {

        public RemoveFromSevenSecondMessage(int price)
        {
            Value = price;
        }

    }

    public class RemoveFromElevenSecondMessage : BaseMessage
    {

        public RemoveFromElevenSecondMessage(int price)
        {
            Value = price;
        }
    }

    public class BaseMessage
    {
        public int Value { get; set; }
    }

    public class BasePublishMessage
    {
        
    }

    public class PublishMessage : BasePublishMessage
    {
        public int Interval { get; set; }
        public int Price { get; set; }
    }

    public class StopLossHitMessage : BasePublishMessage
    {
        
    }
    public class ThresholdUpdatedMessage : BasePublishMessage
    {
        public int ThresholdPrice { get; set; }
    }

    public interface IPublisher
    {
        void Publish(BasePublishMessage message);
    }
}
