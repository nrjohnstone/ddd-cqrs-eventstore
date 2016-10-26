using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Commands
{
    public class CookFood : MessageBase, ITimeToLive
    {
        public RestaurantDocument Order { get; }

        public CookFood(RestaurantDocument order, string messageId, string correlationId, string causativeId, DateTime timeToLive) : base(messageId, correlationId, causativeId)
        {
            Order = order;
            TimeToLive = timeToLive;
        }

        public DateTime TimeToLive { get; }
    }
}
