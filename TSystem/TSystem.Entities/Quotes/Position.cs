using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class Position
    {
        public uint Instrument { get; set; }
        public int Quantity { get; set; }
        public decimal Average { get; set; }
        public decimal LTP { get; set; }
        public TradeType TradeType { get; set; }
        public decimal PnL
        {
            get
            {
                if (TradeType == TradeType.Long)
                {
                    return (LTP - Average);
                }
                else if (TradeType == TradeType.Short)
                {
                    return (Average - LTP);
                }
                return 0m;
            }
        }
        public decimal ChangePercentage { get { return PnL * 100 / Average; } }
    }
}
