using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal interface IOrderHandler<T>
    {
        void Handle(T message);
    }

    internal class MessageBase
    {
        public MessageBase()
        {
        }

        public string Id { get; protected set; }
    }
}