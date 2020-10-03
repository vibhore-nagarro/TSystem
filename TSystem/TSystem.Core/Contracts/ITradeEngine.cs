using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core.Contracts
{
    public interface ITradeEngine
    {
        void PlaceMarketOrder(TradeType tradeType, ProductType productType, int quantity);
        void PlaceLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice, decimal? stoploss = null);
        void PlaceStoplossLimitOrder(TradeType tradeType, ProductType productType, int quantity, decimal limitPrice, decimal triggerPrice, decimal? stoploss = null);

        void ProcessSignal(Signal signal, Analyzer analyzer);
    }
}
