using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Entities;

namespace TSystem.Core
{
    public class OrderBook : IOrderBook
    {
        public static IOrderBook Instance { get; } = new OrderBook();
        public List<Order> Orders { get; set; } = new List<Order>();

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }
        public void RemoveOrder(Order order)
        {
            Orders.Remove(order);
        }

        public void RemoveOrderbyId(long orderId)
        {
            Orders.RemoveAll(order => order.Id == orderId);
        }

        public void ExecuteOrders(decimal price)
        {
            var copyOfOrders = Orders.ToList();
            foreach (Order order in copyOfOrders)
            {
                Order newOrder = order.Execute(price);
                RemoveOrderbyId(order.Id);
                if (!newOrder.IsExecuted)
                    AddOrder(newOrder);
            }
        }
    }
}
