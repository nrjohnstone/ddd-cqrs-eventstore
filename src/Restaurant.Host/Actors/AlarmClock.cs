using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Restaurant.Host.Publishers;

namespace Restaurant.Host.Actors
{
    public class AlarmClock : IOrderHandler<DelayPublish>, IStartable
    {
        private readonly IPublisher _publisher;
        private readonly List<DelayPublish> _delayedMessages;
        private Thread _thread;
        private readonly object _delayedMessagesLock = new object();

        public AlarmClock(IPublisher publisher)
        {
            _publisher = publisher;
            _delayedMessages = new List<DelayPublish>();
        }

        public void Handle(DelayPublish message)
        {
            lock (_delayedMessagesLock)
            {
                _delayedMessages.Add(message);
            }
        }

        public void Start()
        {
            _thread = new Thread(() =>
            {
                while (true)
                {
                    DelayedMessageHandle();
                }
            });
            _thread.Start();
        }

        public void DelayedMessageHandle()
        {
            List<DelayPublish> publishedMessages = new List<DelayPublish>();

            lock (_delayedMessagesLock)
            {
                foreach (var delayedMessage in _delayedMessages)
                {
                    if (delayedMessage.TimeToPublish < Now())
                    {
                        _publisher.Publish(delayedMessage.Message);
                        publishedMessages.Add(delayedMessage);
                    }
                }
            }

            foreach (var publishedMessage in publishedMessages)
            {
                lock (_delayedMessagesLock)
                {
                    _delayedMessages.Remove(publishedMessage);
                }
            }
        }

        protected virtual DateTime Now()
        {
            return DateTime.Now;
        }
    }
}