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
            
            var newSignal = LongEntry(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = ShortEntry(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = LongExit(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
            newSignal = ShortExit(model);
            if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;

            signal.Price = model.LTP;
            signal.TimeStamp = model.Candles.Last().TimeStamp;
            return signal;
        }

        private Signal ShortEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentCandle = model.HeikinAshi.Last();
            uint lastIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();
            var previousCandle = model.HeikinAshi.FirstOrDefault(c => c.Index == lastIndex);

            if (currentCandle.IsRed && previousCandle.IsGreen)
            {
                if (currentCandle.Close < previousCandle.Open)
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;
                    if (currentCandle.HasUpperWick == false)
                    {
                        signal.Strength += 5;
                    }
                    if (previousCandle.HasUpperWick)
                    {
                        signal.Strength += 5;
                        if (previousCandle.UpperWick.Percentage(previousCandle.Body) > 75)
                        {
                            signal.Strength += 5;
                        }
                    }                                        
                    if (currentCandle.HasLowerWick)
                    {
                        signal.Strength += 5;
                        if (currentCandle.LowerWick.Percentage(currentCandle.Body) > 50)
                        {
                            signal.Strength += 5;
                        }
                    }
                    if (currentCandle.Body > previousCandle.Body)
                    {
                        signal.Strength += 10;
                    }

                    signal.Price = currentCandle.Close;
                }
            }
            return signal;
        }

        private Signal LongEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentCandle = model.HeikinAshi.Last();
            uint lastIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();
            var previousCandle = model.HeikinAshi.FirstOrDefault(c => c.Index == lastIndex);

            // Trend Reversal
            if (currentCandle.IsGreen && previousCandle.IsRed)
            {
                // Strong reverse trend
                if (currentCandle.Close > previousCandle.Open)
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;
                    // Only up move
                    if (currentCandle.HasLowerWick == false)
                    {
                        signal.Strength += 5;
                    }
                    // Pulled back up from a low move
                    if (previousCandle.HasLowerWick)
                    {
                        signal.Strength += 5;
                        if (previousCandle.LowerWick.Percentage(previousCandle.Body) > 75)
                        {
                            signal.Strength += 5;
                        }
                    }
                    // Strong up move
                    if (currentCandle.HasUpperWick)
                    {
                        signal.Strength += 5;
                        if (currentCandle.UpperWick.Percentage(currentCandle.Body) > 50)
                        {
                            signal.Strength += 5;
                        }
                    }
                    // Move stronger than previous candle
                    if (currentCandle.Body > previousCandle.Body)
                    {
                        signal.Strength += 10;
                    }

                    signal.Price = currentCandle.Close;
                }
            }

            return signal;
        }

        private Signal LongExit(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentCandle = model.HeikinAshi.Last();
            var previousCandle = model.HeikinAshi.ElementAt(model.HeikinAshi.Count - 2);

            if (currentCandle.IsGreen && previousCandle.IsGreen)
            {
                if (currentCandle.Body < previousCandle.Body && currentCandle.Close < previousCandle.Close && currentCandle.LowerWick > 5)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;

                    signal.Price = currentCandle.Close;
                }
            }

            if(currentCandle.IsRed && previousCandle.IsGreen)
            {
                if (currentCandle.Body < previousCandle.Body || currentCandle.Close < previousCandle.Open)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;

                    signal.Price = currentCandle.Close;
                }
            }

            return signal;
        }

        private Signal ShortExit(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentCandle = model.HeikinAshi.Last();
            var previousCandle = model.HeikinAshi.ElementAt(model.HeikinAshi.Count - 2);

            if (currentCandle.IsRed && previousCandle.IsRed)
            {
                if (currentCandle.Body < previousCandle.Body && currentCandle.Close > previousCandle.Close && currentCandle.UpperWick > 5)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentCandle.Close;
                }
            }

            if (currentCandle.IsGreen && previousCandle.IsRed)
            {
                if (currentCandle.Body < previousCandle.Body || currentCandle.Close > previousCandle.Open)
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentCandle.Close;
                }
            }

            return signal;
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
