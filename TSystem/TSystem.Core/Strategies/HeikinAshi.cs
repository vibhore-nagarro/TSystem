﻿using System;
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
            if (model.HeikinAshi.Count < 3) return signal;
            if (2m > model.HeikinAshi.Last().Body) return signal;
            if (2m > model.Candles.Last().Body) return signal;

            if (model.LeadingHeikinAshi == null) model.LeadingHeikinAshi = model.HeikinAshi.Last();
            if (model.LeadingCandle == null) model.LeadingCandle = model.Candles.Last();

            var currentHeikinAshi = model.CurrentHeikinAshi;
            var currentCandle = model.CurrentCandle;
            var previousHeikinAshi = model.PreviousHeikinAshi;
            var previousCandle = model.PreviousCandle;

            if (currentCandle.TimeStamp.Hour == 10 && currentCandle.TimeStamp.Minute == 50)
            {

            }
            if (currentHeikinAshi.Body.Percentage(model.AverageHeikinAshiBody) > 20 && currentCandle.Body.Percentage(model.AverageCandleBody) > 20)
            {
                var newSignal = LongExit(model);
                if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
                newSignal = ShortExit(model);
                if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
                newSignal = LongEntry(model);
                if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
                newSignal = ShortEntry(model);
                if (newSignal.SignalType != SignalType.None && newSignal.Strength > signal.Strength) signal = newSignal;
                

                signal.Price = model.Candles.Last().Close;
                signal.TimeStamp = model.Candles.Last().TimeStamp;
            }

            UpdateLeadingHeikinAshi(model);
            UpdateLeadingCandle(model);

            if (signal.SignalType == SignalType.Entry && (currentCandle.Body < 5m || currentHeikinAshi.Body < 5m))
                signal = new Signal() { };

            //if (signal.SignalType == SignalType.Entry && (currentCandle.CandleVolume < model.AverageVolume) && model.Candles.Count >= 10)
            //    signal = new Signal() { };

            return signal;
        }
        private Signal LongEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.CurrentHeikinAshi;
            var currentCandle = model.CurrentCandle;
            var previousHeikinAshi = model.PreviousHeikinAshi;
            var previousCandle = model.PreviousCandle;

            // Trend Reversal
            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsRed)
            {
                // Strong reverse trend
                if (currentHeikinAshi.Close > previousHeikinAshi.Open && currentCandle.Body > model.AverageCandleBody && currentHeikinAshi.IsCrossingLeadingCandle(model.LeadingHeikinAshi))
                {
                    if (currentCandle.IsGreen)
                    {
                        signal.SignalType = SignalType.Entry;
                        signal.TradeType = TradeType.Long;
                        signal.Strength = 50;
                    }
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
            }
            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Close > previousHeikinAshi.Close && currentCandle.Body > model.AverageCandleBody)
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;
                    signal.Price = currentHeikinAshi.Close;
                }
            }
            if (currentCandle.IsGreen)
            {
                if (currentCandle.Close > previousCandle.Open && currentCandle.Body > model.AverageCandleBody && currentCandle.IsCrossingLeadingCandle(model.LeadingCandle))
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;
                }
            }

            return signal;
        }

        private Signal ShortEntry(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.CurrentHeikinAshi;
            var currentCandle = model.CurrentCandle;
            var previousHeikinAshi = model.PreviousHeikinAshi;
            var previousCandle = model.PreviousCandle;

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Close < previousHeikinAshi.Open && currentCandle.Body > model.AverageCandleBody && currentHeikinAshi.IsCrossingLeadingCandle(model.LeadingHeikinAshi))
                {
                    if (currentCandle.IsRed)
                    {
                        signal.SignalType = SignalType.Entry;
                        signal.TradeType = TradeType.Short;
                        signal.Strength = 50;
                    }
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
            }
            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Close < previousHeikinAshi.Close && currentCandle.Body > model.AverageCandleBody)
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;
                    signal.Price = currentHeikinAshi.Close;
                }
            }
            if (currentCandle.IsRed)
            {
                if (currentCandle.Close < previousCandle.Open && currentCandle.Body > model.AverageCandleBody && currentCandle.IsCrossingLeadingCandle(model.LeadingCandle))
                {
                    signal.SignalType = SignalType.Entry;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;
                }
            }
            return signal;
        }
        
        private Signal LongExit(AnalysisModel model)
        {
            Signal signal = new Signal();

            var currentHeikinAshi = model.CurrentHeikinAshi;
            var currentCandle = model.CurrentCandle;
            var previousHeikinAshi = model.PreviousHeikinAshi;
            var previousCandle = model.PreviousCandle;

            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body && currentHeikinAshi.Close < previousHeikinAshi.Close
                    && currentHeikinAshi.Body < (currentHeikinAshi.UpperWick + currentHeikinAshi.LowerWick) && currentCandle.IsRed
                    && currentCandle.Close < (previousCandle.IsGreen ? previousCandle.Open : previousCandle.Close))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Long;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsGreen)
            {
                if (currentHeikinAshi.Close < previousHeikinAshi.Open || currentHeikinAshi.Body < previousHeikinAshi.Body 
                    || currentHeikinAshi.IsCrossingLeadingCandle(model.LeadingHeikinAshi))
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

            var currentHeikinAshi = model.CurrentHeikinAshi;
            var currentCandle = model.CurrentCandle;
            var previousHeikinAshi = model.PreviousHeikinAshi;
            var previousCandle = model.PreviousCandle;

            if (currentHeikinAshi.IsRed && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Body < previousHeikinAshi.Body && currentHeikinAshi.Close > previousHeikinAshi.Close
                    && currentHeikinAshi.Body < (currentHeikinAshi.UpperWick + currentHeikinAshi.LowerWick) && currentCandle.IsGreen
                    && currentCandle.Close > (previousCandle.IsGreen ? previousCandle.Close : previousCandle.Open))
                {
                    signal.SignalType = SignalType.Exit;
                    signal.TradeType = TradeType.Short;
                    signal.Strength = 50;

                    signal.Price = currentHeikinAshi.Close;
                }
            }

            if (currentHeikinAshi.IsGreen && previousHeikinAshi.IsRed)
            {
                if (currentHeikinAshi.Close > previousHeikinAshi.Open || currentHeikinAshi.Body < previousHeikinAshi.Body 
                    || currentHeikinAshi.IsCrossingLeadingCandle(model.LeadingHeikinAshi))
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
            var currentHeikinAshi = model.HeikinAshi.Last();
            var lastIndex = model.HeikinAshi.Select(c => c.Index).OrderBy(item => Math.Abs(currentHeikinAshi.Index - item)).Skip(1).First();
            var previousHeikinAshi = model.HeikinAshi.FirstOrDefault(c => c.Index == lastIndex);
            if (currentHeikinAshi.IsCrossingLeadingCandle(model.LeadingHeikinAshi) && currentHeikinAshi.Body.Percentage(model.AverageHeikinAshiBody) < 400)
            {
                model.LeadingHeikinAshi = currentHeikinAshi;
            }
        }

        private static void UpdateLeadingCandle(AnalysisModel model)
        {
            var currentCandle = model.Candles.Last();

            uint lastIndex = model.Candles.Select(c => c.Index).OrderBy(item => Math.Abs(currentCandle.Index - item)).Skip(1).First();
            var previousCandle = model.Candles.FirstOrDefault(c => c.Index == lastIndex);
            if (currentCandle.IsCrossingLeadingCandle(model.LeadingCandle) && currentCandle.Body.Percentage(model.AverageCandleBody) < 400)
            {
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
