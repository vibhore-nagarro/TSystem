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
        Timer timer = new Timer(1*1000*60);
        Timer secondsTimer = new Timer(1000);

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
            timer.Elapsed += Timer_Elapsed;
            secondsTimer.Elapsed += SecondsTimer_Elapsed;            
        }        

        #region Methods

        public void Start()
        {
            secondsTimer.Start();            
        }

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 0)
            {
                timer.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
            }
        }

        internal void ProcessTick(Tick tickData)
        {
            Analyzer analyzer;
            if (analyzers.ContainsKey(tickData.InstrumentToken) == false)
            {
                analyzer = new Analyzer(tickData.InstrumentToken);
                analyzer.CandleRecieved += Analyzer_CandleRecievedEvent;
                analyzer.SignalRecieved += Analyzer_SignalRecievedEvent;

                analyzers.TryAdd(tickData.InstrumentToken, analyzer);
            }
            analyzer = analyzers[tickData.InstrumentToken];

            analyzer.Update(tickData);
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
                Debug.WriteLine($"{signal.TradeType} {signal.SignalType} @ {signal.Price} @ {signal.TimeStamp}");
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();
            //RestRequest request = new RestRequest("https://kite.zerodha.com/oms/portfolio/positions", Method.GET, DataFormat.Json);
            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/55556103/minute?user_id=ZW2177&oi=1&from=2020-09-23&to=2020-09-23&ciqrandom=1600857993840", Method.GET, DataFormat.Json);
            request.AddHeader("authorization", "enctoken 57/ZwEV5PgVpue0TRarT6Tgls+nxNQB6wvTOTWZ7fCaNz4B7V+Php8kfcjKiIN12Wn7HETNWxPTqqHQNH0Dn8ZnkP/r3Tw==");

            dynamic result = JsonConvert.DeserializeObject(client.Execute(request).Content);
            foreach (dynamic candle in result.data.candles)
            {
                candles.Add(new Candle()
                {
                    TimeStamp = candle[0],
                    Open = candle[1],
                    Close = candle[4],
                    High = candle[2],
                    Low = candle[3],
                    Volume = candle[5],
                });
            }
            candles.ElementAt(candles.Count - 2).Print();
            ProcessCandle(candles.ElementAt(candles.Count - 2));
        }

        public void BackTest()
        {
            
        }

        private void Analyzer_SignalRecievedEvent(object sender, SignalRecievedEventArgs e)
        {
        }

        private void Analyzer_CandleRecievedEvent(object sender, CandleRecievedEventArgs e)
        {
        }        

        #endregion
    }
}
