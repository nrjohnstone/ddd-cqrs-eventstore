using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class OrderFactory
    {
        public static RestaurantDocument FishAndChips()
        {
            var order = new RestaurantDocument("1");
            order.Items.Add(new Item("Fish and Chips"));
            return order;
        }

        public static RestaurantDocument Hamburger()
        {
            var order = new RestaurantDocument("2");
            order.Items.Add(new Item("Hamburger"));
            return order;
        }

        public static RestaurantDocument Haggis()
        {
            var order = new RestaurantDocument("3");
            order.Items.Add(new Item("Haggis"));
            return order;
        }

        public static RestaurantDocument Kapsalon()
        {
            var order = new RestaurantDocument("4");
            order.Items.Add(new Item("Kapsalon"));
            return order;
        }
    }
}