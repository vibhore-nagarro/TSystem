using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public struct DepthItem
    {
        public uint Quantity { get; set; }
        public decimal Price { get; set; }
        public uint Orders { get; set; }
    }
    public struct Tick
    {
        public string Mode { get; set; }
        public uint OIDayHigh { get; set; }
        public uint OI { get; set; }
        public DateTime? LastTradeTime { get; set; }
        public DepthItem[] Offers { get; set; }
        public DepthItem[] Bids { get; set; }
        public decimal Change { get; set; }
        public decimal Close { get; set; }
        public decimal Low { get; set; }
        public uint OIDayLow { get; set; }
        public decimal High { get; set; }
        public uint SellQuantity { get; set; }
        public uint BuyQuantity { get; set; }
        public uint Volume { get; set; }
        public decimal AveragePrice { get; set; }
        public uint LastQuantity { get; set; }
        public decimal LastPrice { get; set; }
        public bool Tradable { get; set; }
        public uint InstrumentToken { get; set; }
        public decimal Open { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
