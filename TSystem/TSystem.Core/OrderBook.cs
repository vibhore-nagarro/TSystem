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
        private OrderBook()
        {
        }
        public static IOrderBook Instance { get; } = new OrderBook();
        public List<Order> Orders { get; set; } = new List<Order>();

        public void AddOrder(Order order)
        {
            if(Orders.Count(o => o.TradeType == order.TradeType) == 0)
                Orders.Add(order);
            else
            {
                var exitsingOrder = Orders.First(o => o.TradeType == order.TradeType);
                exitsingOrder.Quantity = order.Quantity;    
                if(exitsingOrder is LimitOrder || order is LimitOrder)
                {
                    LimitOrder limitOrder = order as LimitOrder;
                    LimitOrder exitsingLimitOrder = exitsingOrder as LimitOrder;
                    exitsingLimitOrder.LimitPrice = limitOrder.LimitPrice;
                }
                if (exitsingOrder is StoplossLimitOrder || order is StoplossLimitOrder)
                {
                    StoplossLimitOrder limitOrder = order as StoplossLimitOrder;
                    StoplossLimitOrder exitsingLimitOrder = exitsingOrder as StoplossLimitOrder;
                    exitsingLimitOrder.LimitPrice = limitOrder.LimitPrice;
                    exitsingLimitOrder.TriggerPrice = limitOrder.TriggerPrice;
                }
            }
        }
        public void RemoveOrder(Order order)
        {
            Orders.Remove(order);
        }

        public void RemoveOrderbyId(long orderId)
        {
            Orders.RemoveAll(order => order.Id == orderId);
        }

        public void ExecuteOrders(Candle candle)
        {
            var copyOfOrders = Orders.ToList();
            foreach (Order order in copyOfOrders)
            {
                decimal price = order.TradeType == Entities.Enums.TradeType.Long ? candle.Low : candle.High;

                Order newOrder = order.Execute(price);

                if (newOrder.IsExecuted)
                    RemoveOrderbyId(order.Id);
            }
        }
    }
}
