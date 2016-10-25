using Restaurant.Tests;

namespace Restaurant.Host
{
    internal interface IOrderHandler
    {
        void Handle(RestaurantDocument order);
    }
}