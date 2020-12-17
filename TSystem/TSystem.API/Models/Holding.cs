using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [JsonProperty("average_price")]
        public decimal AveragePrice { get; set; }
        [JsonProperty("last_price")]
        public decimal LastPrice { get; set; }
        public decimal PnL { get; set; }
    }
}
