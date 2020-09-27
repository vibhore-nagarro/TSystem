using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSystem.Common.Extensions;
using TSystem.Core.Contracts;
using TSystem.Core.Models;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core.Strategies
{
    public class HeikinAshi : IStrategy
    {
        const int SAMPLE_SIZE = 5;
        public Signal Apply(AnalysisModel model)
        {
            Signal signal = new Signal();
            if (model.HeikinAshi.Count < 5) return signal;
            if (0.02m > model.HeikinAshi.Last().Body) return signal;
            if (0.02m > model.Candles.Last().Body) return signal;

            if (model.LeadingHeikinAshi == null) model.LeadingHeikinAshi = model.HeikinAshi.Last();
            if (model.LeadingCandle == null) model.LeadingCandle = model.Candles.Last();

            var newSignal = LongEntry(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = ShortEntry(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = LongExit(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = ShortExit(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;            
            
            UpdateLeadingHeikinAshi(model);
            UpdateLeadingCandle(model);

            signal.Price = model.Candles.Last().Close;
            signal.TimeStamp = model.Candles.Last().TimeStamp;
            return signal;
        }
        private Signal LongEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.HeikinAshi.Last();
            var currentCandle = model.Candles.Last();

            uint lastHeikinAshiIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentHeikinAshi.Index - item)).Skip(1).First();
            uint lastIndex = model.Candles.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();

            var previousHeikinAshi = model.HeikinAshi.FirstOrDefault(c => c.Index == lastHeikinAshiIndex);
            var previousCandle = model.Candles.FirstOrDefault(c => c.Index == lastIndex);

            if (currentHeikinAshi.TimeStamp.Day == 23 && currentHeikinAshi.TimeStamp.Month == 4)
            {

            }

            // Trend Reversal
            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsRed)
            {
                // Strong reverse trend
                if (currentHeikinAshi.Close > previousHeikinAshi.Open 
                    && currentHeikinAshi.Close > (model.LeadingHeikinAshi.IsGreen ? model.LeadingHeikinAshi.Close : model.LeadingHeikinAshi.Open))
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;
                    // Only up move
                    if (currentHeikinAshi.HasLowerWick == false)
                    {
                        signal.Strength += 5;
                    }
                    // Pulled back up from a low move
                    if (previousHeikinAshi.HasLowerWick)
                    {
                        signal.Strength += 5;
                        if (previousHeikinAshi.LowerWick.Percentage(previousHeikinAshi.Body) > 75)
                        {
                            signal.Strength += 5;
                        }
                    }
                    // Strong up move
                    if (currentHeikinAshi.HasUpperWick)
                    {
                        signal.Strength += 5;
                        if (currentHeikinAshi.UpperWick.Percentage(currentHeikinAshi.Body) > 50)
                        {
                            signal.Strength += 5;
                        }
                    }
                    // Move stronger than previous candle
                    if (currentHeikinAshi.Body > previousHeikinAshi.Body)
                    {
                        signal.Strength += 10;
                    }

                    signal.Price = currentHeikinAshi.Close;
                }

                if (currentCandle.IsGreen)
                {
                    if (currentCandle.Close > previousCandle.Open && currentCandle.Close > model.LeadingCandle.Open)
                    {
                        signal.SignalType = SignalType.Entry;
                        signal.TradeType = TradeType.Long;
                        signal.Strength = 50;
                    }
                }
            }

            return signal;
        }

        private Signal ShortEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.HeikinAshi.Last();
            var currentCandle = model.Candles.Last();

            uint lastHeikinAshiIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentHeikinAshi.Index - item)).Skip(1).First();
            uint lastIndex = model.Candles.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();

            var previousHeikinAshi = model.HeikinAshi.FirstOrDefault(c => c.Index == lastHeikinAshiIndex);
            var previousCandle = model.Candles.FirstOrDefault(c => c.Index == lastIndex);

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Close < previousHeikinAshi.Open 
                    && currentHeikinAshi.Close < (model.LeadingHeikinAshi.IsGreen ? model.LeadingHeikinAshi.Open : model.LeadingHeikinAshi.Close))
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;
                    if (currentHeikinAshi.HasUpperWick == false)
                    {
                        signal.Strength += 5;
                    }
                    if (previousHeikinAshi.HasUpperWick)
                    {
                        signal.Strength += 5;
                        if (previousHeikinAshi.UpperWick.Percentage(previousHeikinAshi.Body) > 75)
                        {
                            signal.Strength += 5;
                        }
                    }
                    if (currentHeikinAshi.HasLowerWick)
                    {
                        signal.Strength += 5;
                        if (currentHeikinAshi.LowerWick.Percentage(currentHeikinAshi.Body) > 50)
                        {
                            signal.Strength += 5;
                        }
                    }
                    if (currentHeikinAshi.Body > previousHeikinAshi.Body)
                    {
                        signal.Strength += 10;
                    }

                    signal.Price = currentHeikinAshi.Close;
                }                

                if(currentCandle.IsRed)
                {
                    if (currentCandle.Close < previousCandle.Open 
                        && currentCandle.Close < (model.LeadingHeikinAshi.IsGreen ? model.LeadingHeikinAshi.Open : model.LeadingHeikinAshi.Close))
                    {
                        signal.SignalType = SignalType.Entry;
                        signal.TradeType = TradeType.Short;
                        signal.Strength = 50;
                    }                    
                }
            }
            return signal;
        }
        
        private Signal LongExit(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.HeikinAshi.Last();
            var previousHeikinAshi = model.HeikinAshi.ElementAt(model.HeikinAshi.Count - 2);

            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body && currentHeikinAshi.Close < previousHeikinAshi.Close
                    && currentHeikinAshi.Body < (currentHeikinAshi.UpperWick + currentHeikinAshi.LowerWick))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Close < previousHeikinAshi.Open 
                    || currentHeikinAshi.Close < (model.LeadingHeikinAshi.IsGreen ? model.LeadingHeikinAshi.Close : model.LeadingHeikinAshi.Open))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body || currentHeikinAshi.Close < previousHeikinAshi.Open)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            return signal;
        }

        private Signal ShortExit(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.HeikinAshi.Last();
            var previousHeikinAshi = model.HeikinAshi.ElementAt(model.HeikinAshi.Count - 2);

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body && currentHeikinAshi.Close > previousHeikinAshi.Close
                    && currentHeikinAshi.Body < (currentHeikinAshi.UpperWick + currentHeikinAshi.LowerWick))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Close > previousHeikinAshi.Open 
                    || currentHeikinAshi.Close > (model.LeadingHeikinAshi.IsGreen ? model.LeadingHeikinAshi.Close : model.LeadingHeikinAshi.Open))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body || currentHeikinAshi.Close > previousHeikinAshi.Open)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            return signal;
        }

        private static void UpdateLeadingHeikinAshi(AnalysisModel model)
        {
            var currentCandle = model.HeikinAshi.Last();
            var lastIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();
            var previousCandle = model.HeikinAshi.FirstOrDefault(c => c.Index == lastIndex);
            if (model.LeadingHeikinAshi == null)
            {
                model.LeadingHeikinAshi = currentCandle;
            }
            else
            {
                if (currentCandle.IsGreen && model.LeadingHeikinAshi.IsRed && currentCandle.Close >= model.LeadingHeikinAshi.Open)
                    model.LeadingHeikinAshi = currentCandle;
                else if (currentCandle.IsGreen && model.LeadingHeikinAshi.IsGreen && currentCandle.Close >= model.LeadingHeikinAshi.Close)
                    model.LeadingHeikinAshi = currentCandle;
                else if (currentCandle.IsRed && model.LeadingHeikinAshi.IsGreen && currentCandle.Close <= model.LeadingHeikinAshi.Open)
                    model.LeadingHeikinAshi = currentCandle;
                else if (currentCandle.IsRed && model.LeadingHeikinAshi.IsRed && currentCandle.Close <= model.LeadingHeikinAshi.Close)
                    model.LeadingHeikinAshi = currentCandle;
            }
        }

        private static void UpdateLeadingCandle(AnalysisModel model)
        {
            var currentCandle = model.Candles.Last();

            uint lastIndex = model.Candles.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();
            var previousCandle = model.Candles.FirstOrDefault(c => c.Index == lastIndex);
            if (model.LeadingCandle == null)
            {
                model.LeadingCandle = currentCandle;
            }
            else
            {
                if (currentCandle.IsGreen && model.LeadingCandle.IsRed && currentCandle.Close >= model.LeadingCandle.Open)
                    model.LeadingCandle = currentCandle;
                else if (currentCandle.IsGreen && model.LeadingCandle.IsGreen && currentCandle.Close >= model.LeadingCandle.Close)
                    model.LeadingCandle = currentCandle;
                else if (currentCandle.IsRed && model.LeadingCandle.IsGreen && currentCandle.Close <= model.LeadingCandle.Open)
                    model.LeadingCandle = currentCandle;
                else if (currentCandle.IsRed && model.LeadingCandle.IsRed && currentCandle.Close <= model.LeadingCandle.Close)
                    model.LeadingCandle = currentCandle;
            }
        }

        private bool ConfirmPastTrend(AnalysisModel model)
        {
            bool isTrending = false;

            var lastNCandles = model.HeikinAshi.Skip(model.HeikinAshi.Count - SAMPLE_SIZE).Take(SAMPLE_SIZE).ToList();
            int upCandlesCount = lastNCandles.Count(c => c.IsGreen);
            int downCandlesCount = lastNCandles.Count(c => c.IsRed);

            // 1. If all candles are in same trend.
            if (upCandlesCount == 0 || downCandlesCount == 0)
                return true;

            // 2. 
            decimal max = Math.Max(lastNCandles.Max(c => c.Close), lastNCandles.Max(c => c.Open));
            decimal min = Math.Min(lastNCandles.Min(c => c.Close), lastNCandles.Min(c => c.Open));

            // 3. Find longest streak
            int upStreak = GetLongestStreak(lastNCandles, true);
            int downStreak = GetLongestStreak(lastNCandles, false);
            if (upStreak.Percentage(lastNCandles.Count) > 70 || downStreak.Percentage(lastNCandles.Count) > 70)
                return true;

            return isTrending;
        }

        private int GetLongestStreak(List<Candle> candles, bool findUptrend)
        {
            int count = 0;
            int result = 0;

            for (int i = 0; i < candles.Count(); ++i)
            {
                bool direction = findUptrend ? candles[i].IsRed : candles[i].IsGreen;
                if (direction)
                {
                    count = 0;
                }
                else
                {
                    count++;
                    result = Math.Max(result, count);
                }
            }

            return result;
        }
    }
}
