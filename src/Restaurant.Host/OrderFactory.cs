namespace Restaurant.Host
{
    internal class OrderFactory
    {
        public static Order FishAndChips()
        {
            return new Order("Fish & Chips");
        }

        public static Order Hamburger()
        {
            return new Order("Hamburger");
        }

        public static Order Haggis()
        {
            return new Order("Haggis");
        }

        public static Order Kapsalon()
        {
            return new Order("Kapsalon");
        }
    }
}