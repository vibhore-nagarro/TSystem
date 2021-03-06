﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using TSystem.Common;
using TSystem.Core.Contracts;
using TSystem.Core.Events;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    public class AnalysisEngine
    {
        #region Data Members

        private ConcurrentDictionary<uint, Analyzer> analyzers = new ConcurrentDictionary<uint, Analyzer>();

        public event SignalRecievedEventHandler SignalRecieved;
        public event CandleRecievedEventHandler CandleRecieved;
        public event CandleRecievedEventHandler HeikinAshiRecieved;

        private void OnCandleCreated(Candle candle)
        {
            CandleRecieved?.Invoke(this, new CandleRecievedEventArgs(candle));
        }

        private void OnHeikinAshiRecieved(Candle candle)
        {
            HeikinAshiRecieved?.Invoke(this, new CandleRecievedEventArgs(candle));
        }
        
        private void OnSignalCreated(Signal signal)
        {
            SignalRecieved?.Invoke(this, new SignalRecievedEventArgs(signal));
            ProcessSignal(signal);
        }

        IMarketDataEngine marketDataEngine;
        ITradeEngine tradeEngine;
        IRiskEngine riskEngine;

        #endregion

        public AnalysisEngine()
        {
            marketDataEngine = new BackTestHistoricalDataEngine();
            //marketDataEngine = new LiveMarketDataEngine();
            //marketDataEngine = new HistoricalMarketDataEngine();
            tradeEngine = new TradeEngine();
            riskEngine = RiskEngine.Instance;
        }        

        #region Methods

        public void ChangeEngine(IMarketDataEngine engine)
        {
            marketDataEngine.CandleReceived -= MarketData_CandleReceived;
            marketDataEngine = engine;
            Start();
        }

        public void Start()
        {
            marketDataEngine.CandleReceived += MarketData_CandleReceived;
            marketDataEngine.Start();
        }

        private void MarketData_CandleReceived(object sender, CandleReceivedArgs e)
        {
            switch(e.Type)
            {
                case CandleType.Day:
                    ProcessDayCandle(e.Candle);
                    break;
                case CandleType.Minute:
                    ProcessMinuteCandle(e.Candle);
                    break;
                case CandleType.FiveMinute:
                    ProcessMinuteCandle(e.Candle);
                    break;
                default:
                    ProcessMinuteCandle(e.Candle);
                    break;
            }
        }

        private void ProcessDayCandle(Candle candle)
        {
            ProcessCandle(candle);
        }

        private void ProcessMinuteCandle(Candle candle)
        {
            ProcessCandle(candle);
        }


        private void ProcessCandle(Candle candle)
        {
            Analyzer analyzer = GetAnalyzer(candle);

            analyzer.Model.LTP = candle.Close;
            analyzer.Model.Candles.Add(candle);
            analyzer.BuildHekinAshiCandle();

            if (candle.Close < analyzer.Model.PreviousLow)
            {
                analyzer.Model.PreviousLow = candle.Close;
                analyzer.Model.PreviousHigh = decimal.MinValue;
            }

            if (candle.Close > analyzer.Model.PreviousHigh)
            {
                analyzer.Model.PreviousLow = decimal.MaxValue;
                analyzer.Model.PreviousHigh = candle.Close;
            }

            Debug.WriteLine($"High - {analyzer.Model.PreviousHigh}, Low - {analyzer.Model.PreviousLow}");

            OnCandleCreated(candle);
            OnHeikinAshiRecieved(analyzer.Model.HeikinAshi.Last());

            //System.RunAll(analyzer.Model);

            var signal = analyzer.Analyze();

            if (signal.SignalType != SignalType.None)
            {
                var signals = analyzer.Model.Signals;
                if (signals.Count > 0)
                {
                    if ((signals.Last().IsLongEntry() && (signal.IsLongExit() || signal.IsShortEntry()))
                        || (signals.Last().IsLongExit() && (signal.IsLongEntry() || signal.IsShortEntry()))
                        || (signals.Last().IsShortEntry() && (signal.IsShortExit() || signal.IsLongEntry()))
                        || (signals.Last().IsShortExit() && (signal.IsShortEntry() || signal.IsLongEntry())))
                    {
                        signals.Add(signal);
                        OnSignalCreated(signal);
                    }
                }
                else
                {
                    signals.Add(signal);
                    OnSignalCreated(signal);
                }

                //analyzer.Model.Signals.Add(signal);
                //OnSignalCreated(signal);
            }
        }

        private Analyzer GetAnalyzer(Candle candle)
        {
            if (candle.Instrument <= 0)
                throw new ArgumentException("Invalid Instrument Toekn");
            Analyzer analyzer = null;
            if (analyzers.ContainsKey(candle.Instrument))
            {
                analyzer = analyzers[candle.Instrument];
            }
            else
            {
                analyzer = new Analyzer(candle.Instrument);
                analyzers.TryAdd(candle.Instrument, analyzer);
            }

            return analyzer;
        }

        public void BackTest()
        {
            
        }      

        private void ProcessSignal(Signal signal)
        {
            Logger.Log(signal.ToString());
            Analyzer analyzer = analyzers[signal.Instrument];
            //tradeEngine.ProcessSignal(signal, analyzer);
        }

        #endregion
    }
}
