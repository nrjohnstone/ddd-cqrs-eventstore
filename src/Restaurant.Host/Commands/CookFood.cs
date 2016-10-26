using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Host.Documents;

namespace Restaurant.Host.Commands
{
    internal class CookFood : MessageBase, ITimeToLive
    {
        public RestaurantDocument Order { get; }

        public CookFood(RestaurantDocument order, string messageId, string correlationId, string causativeId) : base(messageId, correlationId, causativeId)
        {
            Order = order;
        }

        public DateTime TimeToLive { get; }
    }
}
