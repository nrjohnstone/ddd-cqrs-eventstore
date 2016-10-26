using Restaurant.Host.Documents;

namespace Restaurant.Host
{
    internal interface IMonitorableQueue
    {
        int Count { get; }
        string Name { get; }
    }
}