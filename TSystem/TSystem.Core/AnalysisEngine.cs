using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
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
            marketDataEngine = new HistoricalMarketDataEngine();
            tradeEngine = new TradeEngine();
            riskEngine = RiskEngine.Instance;
        }        

        #region Methods

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
            }
        }

        private void ProcessDayCandle(Candle candle)
        {
            ProcessCandle(candle);
        }

        
        public void ProcessCandle(Candle candle)
        {
            Analyzer analyzer = GetAnalyzer(candle);

            analyzer.Model.LTP = candle.Close;
            analyzer.Model.Candles.Add(candle);
            analyzer.BuildHekinAshiCandle();
            OnCandleCreated(candle);
            OnHeikinAshiRecieved(analyzer.Model.HeikinAshi.Last());

            System.RunAll(analyzer.Model);

            var signal = analyzer.Analyze();

            if (signal.SignalType != SignalType.None)
            {
                analyzer.Model.Signals.Add(signal);
                OnSignalCreated(signal);
            }
        }

        private Analyzer GetAnalyzer(Candle candle)
        {
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
            Analyzer analyzer = analyzers[signal.Instrument];
            tradeEngine.ProcessSignal(signal, analyzer);
        }

        #endregion
    }
}
