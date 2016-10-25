using System;

namespace Restaurant.Host.Documents
{
    public interface ITimeToLive
    {
        DateTime TimeToLive { get; }
    }
}