using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public abstract class Order
    {
        static long id = 1;
        public long Id { get; }
        public int Quantity { get; set; }        
        public ProductType ProductType { get; set; }
        public TradeType TradeType { get; set; }
        public bool IsExecuted { get; set; }
        public decimal? Stoploss { get; set; }
        public decimal? Target { get; set; }
        public virtual Order Execute(decimal price)
        {
            Portfolio.Instance.AddOrUpdatePosition(price, Quantity = this.Quantity, TradeType = this.TradeType);
            this.IsExecuted = true;
            return this;
        }

        public Order()
        {
            Id = id++;
        }
    }
}
