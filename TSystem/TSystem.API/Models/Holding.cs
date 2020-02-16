using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSystem.API.Models
{
    public class Holdings : List<Holding>
    {
        public Holdings()
        {

        }

        public Holdings(IEnumerable<Holding> holdings)
        {
            this.Clear();
            this.AddRange(holdings);
        }
    }

    public class Holding
    {
        public string Tradingsymbol { get; set; }
        public string Exchange { get; set; }        
        public string ISIN { get; set; }
        public string Product { get; set; }
        public uint Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal LastPrice { get; set; }
        public decimal PnL { get; set; }
    }
}
