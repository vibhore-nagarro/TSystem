using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Core.Models;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core.Strategies
{
    public class PreviousDayHighLowBreakout : IStrategy
    {
        public Signal Apply(AnalysisModel model)
        {
            Signal signal = new Signal();
            if (model.Candles.Count < 2) return signal;

            var currentCandle = model.HeikinAshi.Last();
            var previousCandle = model.HeikinAshi.ElementAt(model.Candles.Count - 2);           

            if (currentCandle.Close > previousCandle.High)
            {
                signal.TradeType = TradeType.Long;
                signal.SignalType = SignalType.Entry;
                signal.Price = currentCandle.Close;
            }
            else if (currentCandle.Close < previousCandle.Low)
            {
                signal.TradeType = TradeType.Short;
                signal.SignalType = SignalType.Entry;
                signal.Price = currentCandle.Close;
            }

            signal.Strength = 100;
            signal.TimeStamp = model.Candles.Last().TimeStamp;

            return signal;
        }
    }
}
