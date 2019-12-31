using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Core.Models
{
    public class PerformanceModel
    {
        public decimal PL { get; set; }
        public uint Signals { get; set; }
        public uint Wins { get; set; }
        public uint Losses { get; set; }
        public uint WinningStreak { get; set; }
        public uint LosingStreak { get; set; }
        public decimal MaxGain { get; set; }
        public decimal MaxLoss { get; set; }
        public decimal TotalGain { get; set; }
        public decimal TotalLoss { get; set; }
        public decimal AvgGain { get; set; }
        public decimal AvgLoss { get; set; }
        public decimal MaxDrawdown { get; set; }
        public decimal PeriodOpen { get; set; }
        public decimal PeriodClose { get; set; }
        public decimal PeriodHigh { get; set; }
        public decimal PeriodLow { get; set; } = decimal.MaxValue;
        public decimal PeriodReturn { get; set; }
    }
}
