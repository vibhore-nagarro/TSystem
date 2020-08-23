using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class StoplossOrder : Order
    {
        public decimal TriggerPrice { get; set; }

        public override Order Execute(decimal price)
        {
            if (TradeType == TradeType.Long)
            {
                if (price <= this.TriggerPrice)
                {
                    base.Execute(price);
                }
            }
            else if (TradeType == TradeType.Short)
            {
                if (price >= this.TriggerPrice)
                {
                    base.Execute(price);
                }
            }
            return this;
        }
    }
}
