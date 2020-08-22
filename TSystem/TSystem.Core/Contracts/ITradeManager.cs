using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core.Contracts
{
    public interface ITradeManager
    {
        void PlaceMarketOrder(TradeType tradeType, ProductType productType, uint quantity);
        void PlaceLimitOrder(TradeType tradeType, ProductType productType, uint quantity, decimal limitPrice);
        void PlaceStoplossLimitOrder(TradeType tradeType, ProductType productType, uint quantity, decimal limitPrice);
    }
}
