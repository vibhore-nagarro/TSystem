using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using TSystem.Core.Contracts;
using TSystem.Core.Events;
using TSystem.Core.Models;
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
        private AnalysisModel model = new AnalysisModel();
        private List<IStrategy> strategies = new List<IStrategy>();

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

        public AnalysisModel Model { get { return model; } }

        #endregion

        #region Methods        
        public void Update(Tick tick)
        {
            model.Ticks.Add(tick);
            model.LTP = tick.LastPrice;
            //Debug.WriteLine($"LTP = {tick.LastPrice}");
        }

        public Signal Analyze()
        {
            Signal signal = new Signal() { Price = 0, Type = SignalType.None };
            foreach (IStrategy strategy in strategies)
            {
                signal = strategy.Apply(model);
            }
            return signal;
        }

        private void InitializeStrategies()
        {
            //strategies.Add(new HeikinAshi());
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
                candle.Index = (uint)model.Candles.Count;
                model.Candles.Add(candle);
            }
        }

        public void BuildHekinAshiCandle()
        {
            if (model.Candles.Count == 0)
                return;
            var candle = model.Candles.Last();
            var heikinAshi = new Candle();

            if (model.HeikinAshi.Count == 0)
            {
                heikinAshi.Open = (candle.Open + candle.Close) / 2;
                heikinAshi.Close = (candle.Open + candle.Close + candle.High + candle.Low) / 4.0m;
                heikinAshi.High = candle.High;
                heikinAshi.Low = candle.Low;
            }
            else
            {
                var lastHeikinAshi = model.HeikinAshi.Last();
                heikinAshi.Open = (lastHeikinAshi.Open + lastHeikinAshi.Close) / 2;
                heikinAshi.Close = (candle.Open + candle.Close + candle.High + candle.Low) / 4.0m;
                heikinAshi.High = Math.Max(Math.Max(heikinAshi.Open, heikinAshi.Close), candle.High);
                heikinAshi.Low = Math.Min(Math.Min(heikinAshi.Open, heikinAshi.Close), candle.Low);
            }
            ulong netVolume = candle.Volume;
            if (model.HeikinAshi.Count > 2)
            {
                netVolume = candle.Volume - model.HeikinAshi.ElementAt(model.HeikinAshi.Count - 2).Volume;
            }
            heikinAshi.CandleVolume = netVolume;
            heikinAshi.Volume = candle.Volume;
            heikinAshi.TimeStamp = candle.TimeStamp;

            heikinAshi.Index = (uint)model.HeikinAshi.Count;
            model.HeikinAshi.Add(heikinAshi);
        }

        private Candle GetCurrentCandle(DateTime signalTime, DateTime startTime, DateTime endTime)
        {
            var ticks = model.Ticks.Where(tick => tick.LastTradeTime >= startTime && tick.LastTradeTime <= endTime).ToList();
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
            if (model.Candles.Count > 0)
            {
                netVolume = volume - model.Candles.Last().Volume;
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
            if (model.HeikinAshi.Any())
            {
                model.HeikinAshi.Last().Print();
                model.HeikinAshi.Any();
            }
            if (signal.Type != SignalType.None)
            {
                OnSignalRecieved(signal);
                Debug.WriteLine($"{signal.Type} @ {signal.Price}");
            }
        }
        string fileData = "";
        public void BackTest()
        {
            var signal = Analyze();
            if (signal.Type != SignalType.None)
            {
                var date = model.HeikinAshi.Last().TimeStamp;
                model.Signals.Add(signal);
                Debug.WriteLine($"{signal.Type} @ {signal.Price}");
                fileData = fileData + $"{signal.Type} , {signal.Price}" + Environment.NewLine;
            }
        }

        public void DumpOutput()
        {
            File.WriteAllText(@"D:\StockData\output.csv", fileData);
        }

        #endregion
    }
}
