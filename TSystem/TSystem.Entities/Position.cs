using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public class Position
    {
        public string InstrumentCode { get; set; }
        public int Quantity { get; set; }
        public decimal Average { get; set; }
        public decimal LTP { get; set; }
        public decimal PnL
        {
            get
            {
                if (Quantity > 0)
                {
                    return LTP - Average;
                }
                else if (Quantity < 0)
                {
                    return Average - LTP;
                }
                return 0m;
            }
        }
        public decimal Change { get; set; }
        public decimal ChangePercentage { get; set; }
    }
}
