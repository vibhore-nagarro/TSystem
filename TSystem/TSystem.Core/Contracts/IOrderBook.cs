using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core.Contracts
{
    public interface IOrderBook
    {
        List<Order> Orders { get; set; }
        void AddOrder(Order order);
        void RemoveOrder(Order order);

        void RemoveOrderbyId(long orderId);
        void ExecuteOrders(Candle candle);
    }
}
