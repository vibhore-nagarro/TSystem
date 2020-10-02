using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TSystem.Entities;

namespace TSystem.Core
{
    
    public class LiveMarketDataEngine : IMarketDataEngine
    {
        public const string Token = "enctoken UNt/QY6GDQGNiCrUgw1l9H2kxA6lZ6CQSpVIbKOmLAbtd5ifAEXRnMag0lr2Lf+5MaBRWO/0pnCM2C7Crkq7+E4mBHsRig==";
        public event CandleReceivedEventHandler CandleReceived;
        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        Timer timer1 = new Timer(1 * 1000 * 60);
        Timer timer5 = new Timer(1 * 1000 * 60);
        Timer timer10 = new Timer(1 * 1000 * 60);
        Timer timer15 = new Timer(1 * 1000 * 60);
        Timer secondsTimer = new Timer(1000);

        public void Start()
        {
            InitializeTimers();

            secondsTimer.Start();
        }

        private void InitializeTimers()
        {
            secondsTimer.Elapsed += SecondsTimer_Elapsed;
            timer1.Elapsed += Timer1_Elapsed;            
        }

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 0)
            {
                timer1.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
            }            
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();
            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/690691/minute?user_id=ZW2177&oi=1&from=2020-08-27&to=2020-09-26&ciqrandom=1601103321336", Method.GET, DataFormat.Json);
            request.AddHeader("authorization", Token);

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
            var currentCandle = candles.ElementAt(candles.Count - 2);
            OnCandleReceived(currentCandle, CandleType.Minute);
        }        
    }
}
