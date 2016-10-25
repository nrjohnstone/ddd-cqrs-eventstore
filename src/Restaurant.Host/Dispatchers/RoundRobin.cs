using System;
using System.Collections;
using System.Collections.Generic;

namespace Restaurant.Host.Dispatchers
{
    internal class RoundRobin : IOrderHandler
    {
        private readonly Queue<IOrderHandler> _handlerQueue;

        public RoundRobin(IEnumerable orderHandlers)
        {
            _handlerQueue = new Queue<IOrderHandler>();
            foreach (IOrderHandler orderHandler in orderHandlers)
            {
                _handlerQueue.Enqueue(orderHandler);
            }
        }

        public void Handle(RestaurantDocument order)
        {
            IOrderHandler nextHandler = null;
            try
            {
                nextHandler = _handlerQueue.Dequeue();
                nextHandler.Handle(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _handlerQueue.Enqueue(nextHandler);
            }
        }
    }
}