using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public class Holding
    {
        public string InstrumentCode { get; set; }
        public int Quantity { get; set; }
        public decimal Average { get; set; }
        public decimal LTP { get; set; }
        public decimal PnL { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercentage { get; set; }
    }
}
