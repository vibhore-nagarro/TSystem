using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Entities;

namespace TSystem.Core
{
    public class OrderBook : IOrderBook
    {
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
    }
}
