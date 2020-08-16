using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        const int Seconds = 1000;
        const int Minutes = 1 * 60 * Seconds;

        #endregion

        #region Events

        public event SignalRecievedEventHandler SignalRecieved;
        public event CandleRecievedEventHandler CandleRecieved;

        private void OnSignalRecieved(Signal signal)
        {
            SignalRecieved?.Invoke(this, new SignalRecievedEventArgs(signal));
        }

        private void OnCandleRecieved(Candle candle)
        {
            CandleRecieved?.Invoke(this, new CandleRecievedEventArgs(candle));
        }

        #endregion

        #region Data Members

        private uint instrument;
        private AnalysisModel analysisModel = new AnalysisModel();
        private PerformanceModel performanceModel = new PerformanceModel();
        private List<IStrategy> strategies = new List<IStrategy>();
        private ITradeManager tradeManager = new TradeManager();

        Timer secondsTimer = new Timer(Seconds);
        Timer minutesTimer = new Timer(Minutes);

        #endregion

        #region Constructor

        public Analyzer(uint instrument)
        {
            this.instrument = instrument;

            InitializeStrategies();
            InitializeTimers();
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
            //Debug.WriteLine($"LTP = {tick.LastPrice}");
        }

        public Signal Analyze()
        {
            Signal signal = new Signal() { Price = 0, SignalType = SignalType.None };
            foreach (IStrategy strategy in strategies)
            {
                signal = strategy.Apply(analysisModel);
            }
            return signal;
        }

        private void InitializeStrategies()
        {
            strategies.Add(new HeikinAshi());
        }

        private void InitializeTimers()
        {
            minutesTimer.Elapsed += Minute_Timer_Elapsed;
            secondsTimer.Elapsed += SecondsTimer_Elapsed;
            secondsTimer.Start();
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
            ulong netVolume = candle.Volume;
            if (analysisModel.HeikinAshi.Count > 2)
            {
                netVolume = candle.Volume - analysisModel.HeikinAshi.ElementAt(analysisModel.HeikinAshi.Count - 2).Volume;
            }
            heikinAshi.CandleVolume = netVolume;
            heikinAshi.Volume = candle.Volume;
            heikinAshi.TimeStamp = candle.TimeStamp;

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

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 0)
            {
                minutesTimer.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
            }
        }
        private void Minute_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BuildCandle(e.SignalTime);
            BuildHekinAshiCandle();
            OnCandleRecieved(Model.HeikinAshi.Last());

            var signal = Analyze();
            if (analysisModel.HeikinAshi.Any())
            {
                analysisModel.HeikinAshi.Last().Print();
                analysisModel.HeikinAshi.Any();
            }
            if (signal.SignalType != SignalType.None)
            {
                OnSignalRecieved(signal);
                Debug.WriteLine($"{signal.SignalType} @ {signal.Price}");
            }
        }

        private decimal ComputePerformanceModel(Signal signal)
        {
            decimal pl = 0m;
            if (Model.Signals.Count > 0)
            {
                Signal lastSignal = Model.Signals.Last();
                if (signal.IsLongExit() && lastSignal.IsLongEntry())
                {
                    pl += signal.Price - lastSignal.Price;
                }
                if (signal.IsShortExit() && signal.IsShortEntry())
                {
                    pl += lastSignal.Price - signal.Price;
                }
                if (pl == 0) return pl;

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

                Model.LastTradePL = pl;
            }
            return pl;
        }

        string fileData = "";
        public Signal BackTest()
        {
            //System.ExecuteOrders(Model.LTP);
            var signal = Analyze();

            if (signal.SignalType != SignalType.None)
            {
                decimal pl = ComputePerformanceModel(signal);

                var date = analysisModel.HeikinAshi.Last().TimeStamp;
                analysisModel.Signals.Add(signal);
                Debug.WriteLine($"{signal.SignalType} @ {signal.Price}");
                fileData = fileData + date + $", {signal.TradeType} - {signal.SignalType} , {signal.Price}" + Environment.NewLine;
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
