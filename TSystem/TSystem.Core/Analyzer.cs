using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using TSystem.Core.Contracts;
using TSystem.Core.Events;
using TSystem.Core.Models;
using TSystem.Core.Strategies;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    public class Analyzer
    {
        #region Constants        

        #endregion

        #region Events

        public event SignalRecievedEventHandler SignalRecieved;
        public event CandleRecievedEventHandler CandleRecieved;

        private void OnSignalRecieved(Signal signal)
        {
            SignalRecieved?.Invoke(this, new SignalRecievedEventArgs(signal));
        }

        #endregion

        #region Data Members

        private uint instrument;
        private AnalysisModel analysisModel = new AnalysisModel();
        private PerformanceModel performanceModel = new PerformanceModel();
        private List<IStrategy> strategies = new List<IStrategy>();
        private ITradeEngine tradeManager = new TradeEngine();

        #endregion

        #region Constructor

        public Analyzer(uint instrument)
        {
            this.instrument = instrument;

            InitializeStrategies();
        }

        #endregion

        #region Properties

        public AnalysisModel Model { get { return analysisModel; } }
        public PerformanceModel PerformanceModel { get { return performanceModel; } }

        #endregion

        #region Methods        
        public void Update(Tick tick)
        {
            analysisModel.Ticks.Add(tick);
            analysisModel.LTP = tick.LastPrice;
        }

        public Signal Analyze()
        {
            Signal signal = new Signal() { Price = 0, SignalType = SignalType.None };
            foreach (IStrategy strategy in strategies)
            {
                signal = strategy.Apply(analysisModel);
                //signal = ApplyFilter1(signal);
                //signal = ApplyFilter2(signal);
                signal = ApplyFilter3(signal);

                signal.Instrument = instrument;
            }
            if (signal.SignalType != SignalType.None)
            {
                decimal pl = ComputePerformanceModel(signal);
            }
            
            return signal;
        }

        /// <summary>
        /// Filter signals by avergae candle body/gap (up/down) opening
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        private Signal ApplyFilter1(Signal signal)
        {
            if (analysisModel.Candles.Count < 2)
                return signal;
            var rsi = analysisModel.RSI();
            var avgPrice = analysisModel.AveragePrice().Average();
            var avgCandleBody = analysisModel.AverageCandleBody;
            var avgVol = analysisModel.AverageVolume;

            var currentCandle = analysisModel.Candles.Last();
            var previousCandle = analysisModel.Candles.ElementAt(analysisModel.Candles.Count - 2);
            if (signal.SignalType == SignalType.Entry)
            {
                // Reset signal if the deciding candle was too small
                if (currentCandle.Body < (avgCandleBody / 2))
                    signal = new Signal() { Price = 0, SignalType = SignalType.None };

                // Reset signal if it opened with gap up/down
                if ((currentCandle.Open - previousCandle.Open) > avgCandleBody)
                    signal = new Signal() { Price = 0, SignalType = SignalType.None };
            }

            return signal;
        }

        /// <summary>
        /// Filter signals by avergae candle body/gap (up/down) opening
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        private Signal ApplyFilter2(Signal signal)
        {
            if (analysisModel.Candles.Count < 5)
                return signal;

            if (signal.IsLongEntry())
            {
                if(Model.CandleTrend(3) == Trend.Up)
                {
                    signal = new Signal() { Price = 0, SignalType = SignalType.None };
                }
            }
            if (signal.IsShortEntry())
            {
                if (Model.CandleTrend(3) == Trend.Down)
                {
                    signal = new Signal() { Price = 0, SignalType = SignalType.None };
                }
            }

            return signal;
        }

        private Signal ApplyFilter3(Signal signal)
        {
            if (analysisModel.Candles.Count < 2)
                return signal;

            // Current candle color must match the direction of signal
            if (signal.IsLongEntry() && Model.CurrentCandle.IsGreen == false)
                signal = new Signal() { Price = 0, SignalType = SignalType.None };
            if (signal.IsShortEntry() && Model.CurrentCandle.IsRed == false)
                signal = new Signal() { Price = 0, SignalType = SignalType.None };

            return signal;
        }

        private void InitializeStrategies()
        {
            strategies.Add(new HeikinAshi());
            //strategies.Add(new PreviousDayHighLowBreakout());            
        }

        private void BuildCandle(DateTime signalTime)
        {
            var startTime = signalTime.AddMinutes(-1);
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
            var endTime = new DateTime(signalTime.Year, signalTime.Month, signalTime.Day, signalTime.Hour, signalTime.Minute, 0);

            Candle candle = GetCurrentCandle(signalTime, startTime, endTime);
            if (candle != null)
            {
                candle.Index = (uint)analysisModel.Candles.Count;
                analysisModel.Candles.Add(candle);
            }
        }

        public void BuildHekinAshiCandle()
        {
            if (analysisModel.Candles.Count == 0)
                return;
            var candle = analysisModel.Candles.Last();
            var heikinAshi = new Candle();

            if (analysisModel.HeikinAshi.Count == 0)
            {
                heikinAshi.Open = (candle.Open + candle.Close) / 2;
                heikinAshi.Close = (candle.Open + candle.Close + candle.High + candle.Low) / 4.0m;
                heikinAshi.High = candle.High;
                heikinAshi.Low = candle.Low;
            }
            else
            {
                var lastHeikinAshi = analysisModel.HeikinAshi.Last();
                heikinAshi.Open = (lastHeikinAshi.Open + lastHeikinAshi.Close) / 2;
                heikinAshi.Close = (candle.Open + candle.Close + candle.High + candle.Low) / 4.0m;
                heikinAshi.High = Math.Max(Math.Max(heikinAshi.Open, heikinAshi.Close), candle.High);
                heikinAshi.Low = Math.Min(Math.Min(heikinAshi.Open, heikinAshi.Close), candle.Low);
            }
            heikinAshi.CandleVolume = candle.CandleVolume;
            heikinAshi.Volume = candle.Volume;
            heikinAshi.TimeStamp = candle.TimeStamp;
            heikinAshi.Instrument = candle.Instrument;
            heikinAshi.Index = (uint)analysisModel.HeikinAshi.Count;
            analysisModel.HeikinAshi.Add(heikinAshi);
        }

        private Candle GetCurrentCandle(DateTime signalTime, DateTime startTime, DateTime endTime)
        {
            var ticks = analysisModel.Ticks.Where(tick => tick.LastTradeTime >= startTime && tick.LastTradeTime <= endTime).ToList();
            if (ticks.Any() == false)
                return null;

            var open = ticks.First().LastPrice;
            var close = ticks.Last().LastPrice;
            var high = ticks.Max(tick => tick.LastPrice);
            var low = ticks.Min(tick => tick.LastPrice);
            var volume = ticks.Last().Volume;

            var candle = new Candle()
            {
                Open = open,
                Close = close,
                High = high,
                Low = low,
                Volume = volume,
                TimeStamp = signalTime
            };

            ulong netVolume = volume;
            if (analysisModel.Candles.Count > 0)
            {
                netVolume = volume - analysisModel.Candles.Last().Volume;
            }
            candle.CandleVolume = netVolume;

            return candle;
        }

        public void ProcessCandle(Candle candle)
        {
            Model.Candles.Add(candle);
            BuildHekinAshiCandle();

            var signal = Analyze();

            
            if (signal.SignalType != SignalType.None)
            {
                OnSignalRecieved(signal);                               
            }
        }

        private decimal ComputePerformanceModel(Signal signal)
        {
            decimal pl = 0m;
            if (Model.Signals.Count > 0)
            {
                Signal lastSignal = Model.Signals.Last();
                if (lastSignal.IsLongEntry() && (signal.IsLongExit() || signal.IsShortEntry()))
                {
                    pl += signal.Price - lastSignal.Price;
                }
                if (lastSignal.IsShortEntry() && (signal.IsShortExit() || signal.IsLongEntry() ))
                {
                    pl += lastSignal.Price - signal.Price;
                }
                if (pl == 0)
                {
                    return pl;
                }

                this.PerformanceModel.PL += pl;
                this.PerformanceModel.Signals++;
                if (pl > 0)
                    this.PerformanceModel.Wins++;
                else
                    this.PerformanceModel.Losses++;
                this.PerformanceModel.WinRate = Decimal.Round((this.PerformanceModel.Wins * 100.0m) / this.PerformanceModel.Signals, 2);
                this.PerformanceModel.LossRate = Decimal.Round((this.PerformanceModel.Losses * 100.0m) / this.PerformanceModel.Signals, 2);
                if (Model.LastTradePL >= 0 && pl > 0)
                    this.PerformanceModel.WinningStreak++;
                else if(Model.LastTradePL >= 0 && pl < 0)
                    this.PerformanceModel.LosingStreak++;
                if (this.PerformanceModel.MaxGain < pl)
                    this.PerformanceModel.MaxGain = pl;
                if (this.PerformanceModel.MaxLoss > pl)
                    this.PerformanceModel.MaxLoss = pl;
                if (pl > 0)
                    this.PerformanceModel.TotalGain += pl;
                if (pl < 0)
                    this.PerformanceModel.TotalLoss += pl;
                this.PerformanceModel.AvgGain = this.PerformanceModel.Wins > 0 ? this.PerformanceModel.TotalGain / this.PerformanceModel.Wins : 0;
                this.PerformanceModel.AvgLoss = this.PerformanceModel.Losses > 0 ? this.PerformanceModel.TotalLoss / this.PerformanceModel.Losses : 0;
                this.PerformanceModel.PeriodOpen = this.Model.Candles.First().Open;
                this.PerformanceModel.PeriodClose = this.Model.Candles.Last().Close;
                if (this.PerformanceModel.PeriodHigh < this.Model.Candles.Last().High)
                    this.PerformanceModel.PeriodHigh = this.Model.Candles.Last().High;
                if (this.PerformanceModel.PeriodLow > this.Model.Candles.Last().Low)
                    this.PerformanceModel.PeriodLow = this.Model.Candles.Last().Low;
                this.PerformanceModel.PeriodReturn = ((this.PerformanceModel.PeriodClose - this.PerformanceModel.PeriodOpen) * 100) / this.PerformanceModel.PeriodOpen;

                this.PerformanceModel.LastTradePL = pl;
                Model.LastTradePL = pl;
            }
            return pl;
        }

        string fileData = "";
        public Signal BackTest()
        {
            //System.RunAll(analysisModel);

            var signal = Analyze();
            var lastSignal = analysisModel.Signals.LastOrDefault();

            if (signal.SignalType != SignalType.None)
            {
                if (lastSignal == null || ((lastSignal.IsLongEntry() && (signal.IsLongExit() || signal.IsShortEntry())) || 
                     lastSignal.IsShortEntry() && (signal.IsShortExit() || signal.IsLongEntry()) ||
                     lastSignal.IsLongExit() && (signal.IsLongEntry() || signal.IsShortEntry())   ||
                     lastSignal.IsShortExit() && (signal.IsShortEntry() || signal.IsLongEntry())))
                {
                    System.RunAll(analysisModel);

                    int quanity = 1;
                    tradeManager.PlaceStoplossLimitOrder(signal.TradeType, ProductType.MIS, quanity, signal.Price, signal.Price);

                    decimal pl = ComputePerformanceModel(signal);

                    var date = analysisModel.HeikinAshi.Last().TimeStamp;
                    analysisModel.Signals.Add(signal);
                    
                    fileData = fileData + date + $", {signal.TradeType} - {signal.SignalType} , {signal.Price}" + Environment.NewLine;
                }
                else
                {
                    signal = new Signal() { SignalType = SignalType.None, TradeType = TradeType.None };
                }
            }            
            return signal;
        }

        public void DumpOutput()
        {
            File.WriteAllText(@"D:\StockData\output.csv", fileData);
        }

        #endregion
    }
}
