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
        public void PlaceLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice)
        {
            OrderBook.Instance.AddOrder(new LimitOrder(limitPrice) { TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
        public void PlaceStoplossLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice, decimal triggerPrice)
        {
            OrderBook.Instance.AddOrder(new StoplossLimitOrder(limitPrice) { TriggerPrice = triggerPrice, TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
    }
}
