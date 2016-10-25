using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal interface IOrderHandler
    {
        void Handle(RestaurantDocument order);
    }
}