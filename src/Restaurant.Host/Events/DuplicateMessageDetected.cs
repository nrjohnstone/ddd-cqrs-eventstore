namespace Restaurant.Host.Events
{
    public class DuplicateMessageDetected : MessageBase
    {
        private readonly string _messageType;

        public DuplicateMessageDetected(string messageType, string messageId, string correlationId, string causativeId) :
            base(messageId, correlationId, causativeId)
        {
            _messageType = messageType;
        }
    }
}