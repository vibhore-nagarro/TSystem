using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class Signal
    {
        public SignalType Type { get; set; }
        public int Strength { get; set; }
        public decimal Price { get; set; }
    }
}
