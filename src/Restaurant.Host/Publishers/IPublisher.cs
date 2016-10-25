using Restaurant.Host.Documents;

namespace Restaurant.Host.Publishers
{
    internal interface IPublisher
    {
        void Publish(string topic, RestaurantDocument order);
        void Subscribe(string topic, IOrderHandler handler);
    }
}