using System;

namespace Restaurant.Host.Actors
{
    public class DelayPublish : MessageBase
    {
        public DateTime TimeToPublish { get; set; }
        public MessageBase Message { get; set; }

        public DelayPublish(string messageId, string correlationId, string causativeId,
            DateTime timeToPublish, MessageBase message) : base(messageId, correlationId, causativeId)
        {
            TimeToPublish = timeToPublish;
            Message = message;
        }
    }
}