using System;
using NSubstitute;
using Restaurant.Host;
using Restaurant.Host.Actors;
using Restaurant.Host.Commands;
using Restaurant.Host.Publishers;
using Xunit;

namespace Restaurant.Tests
{
    public class AlarmClockTests
    {
        public class AlarmClockTestDouble : AlarmClock
        {
            protected override DateTime Now()
            {
                return TheDate;
            }

            public DateTime TheDate { get; set; }

            public AlarmClockTestDouble(IPublisher publisher) : base(publisher)
            {
            }
        }

        private IPublisher _mockPublisher;

        public AlarmClockTests()
        {
            _mockPublisher = Substitute.For<IPublisher>();
        }

        [Fact]
        public void ShouldPublishMessage()
        {
            var sut = CreateSut();

            MessageBase messageToBePublished = new CookFood(
                null, "", "", "", DateTime.Now);

            sut.Handle(new DelayPublish(messageToBePublished.MessageId,
                messageToBePublished.CorrelationId, messageToBePublished.CausativeId,
                DateTime.Parse("1-Jan-2016 10:00:00"), messageToBePublished
                ));

            sut.TheDate = DateTime.Parse("1-Jan-2016 10:00:01");

            sut.DelayedMessageHandle();

            _mockPublisher.Received().Publish(messageToBePublished);
        }

        [Fact]
        public void WhenTimeToBePublished_HasNotBeReached_ShouldNotPublishMessage()
        {
            var sut = CreateSut();

            MessageBase messageToBePublished = new CookFood(
                null, "", "", "", DateTime.Now);

            sut.Handle(new DelayPublish(messageToBePublished.MessageId,
                messageToBePublished.CorrelationId, messageToBePublished.CausativeId,
                DateTime.Parse("1-Jan-2016 10:00:01"), messageToBePublished
                ));

            sut.TheDate = DateTime.Parse("1-Jan-2016 10:00:00");

            sut.DelayedMessageHandle();

            _mockPublisher.DidNotReceive().Publish(messageToBePublished);
        }


        [Fact]
        public void OnceMessageIsPublished_IsNotPublishedAgain()
        {
            var sut = CreateSut();

            MessageBase messageToBePublished = new CookFood(
                null, "", "", "", DateTime.Now);

            sut.Handle(new DelayPublish(messageToBePublished.MessageId,
                messageToBePublished.CorrelationId, messageToBePublished.CausativeId,
                DateTime.Parse("1-Jan-2016 10:00:00"), messageToBePublished));

            sut.TheDate = DateTime.Parse("1-Jan-2016 10:00:01");

            sut.DelayedMessageHandle();
            _mockPublisher.ClearReceivedCalls();

            sut.DelayedMessageHandle();
            _mockPublisher.DidNotReceive().Publish(messageToBePublished);
        }

        private AlarmClockTestDouble CreateSut()
        {
            return new AlarmClockTestDouble(_mockPublisher);
        }
    }
}