using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    public class TradeManager : ITradeManager
    {
        IRiskManager riskManager = new RiskManager();        

        public void PlaceMarketOrder(TradeType tradeType, ProductType productType, uint quantity)
        {
            System.OrderBook.AddOrder(new MarketOrder() { TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
        public void PlaceLimitOrder(TradeType tradeType, ProductType productType, uint quantity, decimal limitPrice)
        {
            System.OrderBook.AddOrder(new LimitOrder(limitPrice) { TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
        public void PlaceStoplossLimitOrder(TradeType tradeType, ProductType productType, uint quantity, decimal limitPrice)
        {
            System.OrderBook.AddOrder(new StoplossLimitOrder(limitPrice) { TradeType = tradeType, ProductType = productType, Quantity = quantity });
        }
    }
}
