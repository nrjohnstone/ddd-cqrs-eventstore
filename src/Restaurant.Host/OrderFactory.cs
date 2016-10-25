using Restaurant.Tests;

namespace Restaurant.Host
{
    internal class OrderFactory
    {
        private static int _nextId = 0;

        private static string GetNextId()
        {
            return (++_nextId).ToString();
        }
        public static RestaurantDocument FishAndChips()
        {
            var order = new RestaurantDocument(GetNextId());
            order.Items.Add(new Item("Fish and Chips"));
            return order;
        }

        public static RestaurantDocument Hamburger()
        {
            var order = new RestaurantDocument(GetNextId());
            order.Items.Add(new Item("Hamburger"));
            return order;
        }

        public static RestaurantDocument Haggis()
        {
            var order = new RestaurantDocument(GetNextId());
            order.Items.Add(new Item("Haggis"));
            return order;
        }

        public static RestaurantDocument Kapsalon()
        {
            var order = new RestaurantDocument(GetNextId());
            order.Items.Add(new Item("Kapsalon"));
            return order;
        }
    }
}