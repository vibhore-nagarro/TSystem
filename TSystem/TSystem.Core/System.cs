using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;
using TSystem.Core.Models;

namespace TSystem.Core
{
    public static class System
    {
        public static OrderBook OrderBook { get; } = new OrderBook();
        public static Portfolio Portfolio { get; } = new Portfolio();
        public static Funds Funds { get; } = new Funds();

        public static void RunAll(AnalysisModel model)
        {
            OrderBook.Instance.ExecuteOrders(model.LTP);
            Portfolio.Instance.Positions.ForEach(p => p.LTP = model.LTP);
            RiskManager.Instance.Run(model);
        }
    }
}
