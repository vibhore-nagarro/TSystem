using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core
{
    public enum CandleType
    {
        Minute,
        FiveMinute,
        TenMinute,
        FifteenMinute,
        Hour,
        Day,
    }

    public class CandleReceivedArgs : EventArgs
    {
        public CandleType Type { get; set; }
        public Candle Candle { get; set; }
    }

    public delegate void CandleReceivedEventHandler(object sender, CandleReceivedArgs e);
    public interface IMarketDataManager
    {        
        public event CandleReceivedEventHandler CandleReceived;

        void Start();
    }
}
