﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public class MarketOrder : Order
    {
        public override Order Execute(decimal price)
        {
            base.Execute(price);
            return this;
        }
    }
}
