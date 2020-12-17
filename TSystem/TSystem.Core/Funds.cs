using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Core
{
    public class Funds
    {
        public decimal MarginAvailable { get; set; } = 100000m;
        public decimal MarginUsed { get; set; } = 0m;
        public decimal Total { get { return MarginAvailable + MarginUsed; } }
    }
}
