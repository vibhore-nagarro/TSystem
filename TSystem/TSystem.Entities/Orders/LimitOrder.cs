﻿using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class LimitOrder : Order
    {
        public decimal LimitPrice { get; private set; }
        public LimitOrder(decimal price)
        {
            LimitPrice = price;
        }

        public override Order Execute(decimal price)
        {
            if(TradeType == TradeType.Long)
            {
                if (price <= this.LimitPrice) IsExecuted = true;
            }
            else if (TradeType == TradeType.Short)
            {
                if (price >= this.LimitPrice) IsExecuted = true;
            }
            return this;
        }
    }
}
