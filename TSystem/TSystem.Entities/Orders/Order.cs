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
        public uint Quantity { get; set; }        
        public ProductType ProductType { get; set; }
        public TradeType TradeType { get; set; }
        public bool IsExecuted { get; set; }

        public abstract Order Execute(decimal price);

        public Order()
        {
            Id = id++;
        }
    }
}
