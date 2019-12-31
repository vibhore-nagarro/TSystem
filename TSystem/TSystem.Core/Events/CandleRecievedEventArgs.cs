using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core.Events
{
    public delegate void CandleRecievedEventHandler(object sender, CandleRecievedEventArgs e);
    public class CandleRecievedEventArgs : EventArgs
    {
        public Candle Candle { get; set; }
        public CandleRecievedEventArgs(Candle candle)
        {
            Candle = candle;
        }
    }
}
