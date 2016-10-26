using System;
using System.Collections.Generic;

namespace Restaurant.Host.Documents
{
    public class RestaurantDocument : ITimeToLive
    {
        public string Id { get; }
        public int TableNumber { get; set; }
        public int TimeToCookMs { get; set; }
        public double Tax { get; set; }
        public List<Item> Items { get; set; }
        public List<string> Ingredients { get; set; }
        public double Total { get; set; }
        public double Paid { get; set; }

        public RestaurantDocument(string id, int timeToLiveMs = 5000)
        {
            Id = id;
            Items = new List<Item>();
            Ingredients = new List<string>();
            TimeToLive = DateTime.Now + TimeSpan.FromMilliseconds(timeToLiveMs);
        }

        public DateTime TimeToLive { get; }
        public bool IsDodgy { get; set; }
    }
}