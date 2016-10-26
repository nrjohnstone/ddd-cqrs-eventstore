using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal interface IOrderHandler<T>
    {
        void Handle(T message);
    }

    internal class MessageBase
    {
        public MessageBase(string messageId, string correlationId, string causativeId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausativeId = causativeId;
        }

        public string MessageId { get; protected set; }
        public string CorrelationId { get; }
        public string CausativeId { get; }
    }
}