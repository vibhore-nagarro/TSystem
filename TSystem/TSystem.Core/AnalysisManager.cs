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
using TSystem.Core.Events;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    public class AnalysisManager
    {
        #region Data Members

        private ConcurrentDictionary<uint, Analyzer> analyzers = new ConcurrentDictionary<uint, Analyzer>();

        public event EventHandler CandleCreated;

        private void OnCandleCreated()
        {
            CandleCreated?.Invoke(this, new EventArgs());
        }

        public event EventHandler SignalCreated;

        private void OnSignalCreated()
        {
            SignalCreated?.Invoke(this, new EventArgs());
        }

        #endregion

        public AnalysisManager()
        {         
        }        

        #region Methods

        public void Start()
        {
        }

        public Analyzer analyzer = new Analyzer(1);
        public void ProcessCandle(Candle candle)
        {
            analyzer.Model.Candles.Add(candle);
            analyzer.BuildHekinAshiCandle();
            OnCandleCreated();
            
            var signal = analyzer.Analyze();
            
            if (signal.SignalType != SignalType.None)
            {
                analyzer.Model.Signals.Add(signal);
                OnSignalCreated();
            }
        }


        public void BackTest()
        {
            
        }      

        #endregion
    }
}
