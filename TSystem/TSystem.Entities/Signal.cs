using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class Signal
    {
        public SignalType Type { get; set; } = SignalType.None;
        public int Strength { get; set; } = 0;
        public decimal Price { get; set; } = 0m;
    }
}
