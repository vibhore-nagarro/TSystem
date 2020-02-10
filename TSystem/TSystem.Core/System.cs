using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core
{
    public static class System
    {
        private static OrderBook orderBook = new OrderBook();
        private static Portfolio portfolio = new Portfolio();
        private static Funds funds = new Funds();

        public static OrderBook OrderBook { get { return orderBook; } }
        public static Portfolio Portfolio { get { return portfolio; } }
        public static Funds Funds { get { return funds; } }

        public static void ExecuteOrders(decimal price)
        {
            var copyOfOrders = OrderBook.Orders.ToList();
            foreach (Order order in copyOfOrders)
            {
                Order newOrder = order.Execute(price);
                OrderBook.RemoveOrderbyId(order.Id);
                if (!newOrder.IsExecuted)
                    OrderBook.AddOrder(newOrder);
            }
        }
    }
}
