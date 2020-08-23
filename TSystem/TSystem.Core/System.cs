using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;
using TSystem.Core.Models;
using TSystem.Core.Contracts;

namespace TSystem.Core
{
    public static class System
    {
        public static IOrderBook OrderBook { get; } = TSystem.Core.OrderBook.Instance;
        public static Portfolio Portfolio { get; } = new Portfolio();
        public static Funds Funds { get; } = new Funds();

        public static void RunAll(AnalysisModel model)
        {
            OrderBook.ExecuteOrders(model.Candles.Last().Low);
            Portfolio.Instance.Positions.ForEach(p => p.LTP = model.LTP);
            RiskManager.Instance.Run(model);
        }
    }
}
