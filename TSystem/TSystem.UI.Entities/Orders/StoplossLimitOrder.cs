using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class StoplossLimitOrder : LimitOrder
    {
        public StoplossLimitOrder(decimal price) : base(price)
        {

        }

        public decimal TriggerPrice { get; set; }

        public override Order Execute(decimal price)
        {
            if (TradeType == TradeType.Long)
            {
                if (price <= this.TriggerPrice)
                {
                    return new LimitOrder(this.LimitPrice) { ProductType = this.ProductType, TradeType = this.TradeType, Quantity = this.Quantity, IsExecuted = false }.Execute(price);                    
                }
            }
            else if (TradeType == TradeType.Short)
            {
                if (price >= this.TriggerPrice)
                {
                    return new LimitOrder(this.LimitPrice) { ProductType = this.ProductType, TradeType = this.TradeType, Quantity = this.Quantity, IsExecuted = false }.Execute(price);
                }
            }
            return this;
        }
    }
}
