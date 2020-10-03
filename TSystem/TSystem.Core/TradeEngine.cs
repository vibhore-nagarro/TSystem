using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    public class TradeEngine : ITradeEngine
    {
        IRiskEngine riskManager = RiskEngine.Instance;


        public void PlaceMarketOrder(TradeType tradeType, ProductType productType, int quantity)
        {
            OrderBook.Instance.AddOrder(new MarketOrder() { TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
        public void PlaceLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice, decimal? stoploss = null)
        {
            OrderBook.Instance.AddOrder(new LimitOrder(limitPrice) { TradeType = tradeType, ProductType = productType, Quantity = quantity });
            if (stoploss.HasValue)
            {
                OrderBook.Instance.AddOrder(new LimitOrder(limitPrice) { TradeType = tradeType == TradeType.Long ? TradeType.Short : TradeType.Long, ProductType = productType, Quantity = quantity });
            }
        }
        public void PlaceStoplossLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice, decimal triggerPrice, decimal? stoploss = null)
        {
            OrderBook.Instance.AddOrder(new StoplossLimitOrder(limitPrice) { TriggerPrice = triggerPrice, TradeType = tradeType, ProductType = productType, Quantity = quantity });
            if(stoploss.HasValue)
            {
                OrderBook.Instance.AddOrder(new StoplossLimitOrder(limitPrice) { TriggerPrice = triggerPrice, TradeType = tradeType == TradeType.Long ? TradeType.Short : TradeType.Long, ProductType = productType, Quantity = quantity });
            }
        }

        public void ProcessSignal(Signal signal, Analyzer analyzer)
        {
            var existingPosition = System.Portfolio.Positions.FirstOrDefault(p => p.Instrument == signal.Instrument);
            var existingOrders = OrderBook.Instance.Orders.FirstOrDefault(o => o.TradeType == signal.TradeType);
            if (existingOrders == null)
            {
                if (existingPosition != null)
                {
                    if ((existingPosition.TradeType == TradeType.Long && (signal.IsLongExit() || signal.IsShortEntry()))
                        || existingPosition.TradeType == TradeType.Short && (signal.IsLongEntry() || signal.IsShortExit()))
                    {
                        PlaceLimitOrder(signal.TradeType, ProductType.MIS, 1, signal.Price, signal.Price);
                    }
                }
                else
                {
                    PlaceLimitOrder(signal.TradeType, ProductType.MIS, 1, signal.Price, signal.Price);
                }
            }
        }
    }
}
