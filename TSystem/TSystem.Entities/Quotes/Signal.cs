using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class Signal
    {
        public uint Instrument { get; set; }
        public SignalType SignalType { get; set; } = SignalType.None;
        public TradeType TradeType { get; set; } = TradeType.None;
        public int Strength { get; set; } = 0;
        public decimal Price { get; set; } = 0m;
        public decimal? StopLoss { get; set; } = null;
        public DateTime TimeStamp { get; set; }
        public bool IsDualSignal { get; set; }
        public bool IsLongEntry()
        {
            return this.SignalType == SignalType.Entry && this.TradeType == TradeType.Long;
        }
        public bool IsLongExit()
        {
            return this.SignalType == SignalType.Exit && this.TradeType == TradeType.Long;
        }
        public bool IsShortEntry()
        {
            return this.SignalType == SignalType.Entry && this.TradeType == TradeType.Short;
        }
        public bool IsShortExit()
        {
            return this.SignalType == SignalType.Exit && this.TradeType == TradeType.Short;
        }

        public override string ToString()
        {
            return $"T={TimeStamp.ToShortTimeString()}, Type= {TradeType} {SignalType}, Price= {Price}, Strength= {Strength}";
        }
    }
}
