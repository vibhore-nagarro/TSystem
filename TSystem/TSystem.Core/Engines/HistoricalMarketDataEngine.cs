using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TSystem.Common;
using TSystem.Entities;

namespace TSystem.Core
{
    public class HistoricalMarketDataEngine : IMarketDataEngine
    {
        public const string Token = "enctoken pDm8HGdrHTFdvlT3AZG7lW468cIPAfFMMlGilsCyB/9GeDpgqsBNG7BuNz8Sgk3KQcEkSEDs0mS22nkKHbkkDwaj2Jm9Gw==";
        public event CandleReceivedEventHandler CandleReceived;

        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        Timer timer1 = new Timer(1 * 1000 * 60);
        Timer timer3 = new Timer(3 * 1000 * 60);
        Timer timer5 = new Timer(5 * 1000 * 60);
        Timer timer10 = new Timer(10 * 1000 * 60);
        Timer timer15 = new Timer(15 * 1000 * 60);
        Timer secondsTimer = new Timer(1000);

        public void Start()
        {
            KiteEngine.Instance.Initialize();
            TickEngine.Instance.Start();
            InitializeTimers();

            secondsTimer.Start();
        }

        private void InitializeTimers()
        {
            secondsTimer.Elapsed += SecondsTimer_Elapsed;
            //timer1.Elapsed += Timer1_Elapsed;
            //timer3.Elapsed += Timer3_Elapsed;
            timer5.Elapsed += Timer5_Elapsed;
        }

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 1 && e.SignalTime.Minute % 5 == 0)
            //if (e.SignalTime.Second == 0)
            {
                //timer1.Start();
                //timer3.Start();
                timer5.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
                Logger.Log("Timer started");
            }
        }

        long lastVolume = -1;

        private void Timer5_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessTimerTick(e, CandleType.Minute);
        }

        private void ProcessTimerTick(ElapsedEventArgs e, CandleType candleType)
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();

            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/11984386/5minute?user_id=ZW2177&oi=1&from=2020-12-01&to=2020-12-01&ciqrandom=1606573753955", Method.GET, DataFormat.Json);

            request.AddHeader("authorization", Token);

            dynamic result = JsonConvert.DeserializeObject(client.Execute(request).Content);
            if(result.status == "error" && result.error_type == "TokenException")
            {
                Logger.Log("Token Expired");
                return;
            }
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
                    CandleVolume = candle[5],
                    Instrument = 11984386,
                });                
            }
            OnCandleReceived(candles.Last(), CandleType.FiveMinute);
        }

        private static int GetMinutesToTrack(CandleType candleType)
        {
            int backTrackMinutes = -1;
            if (candleType == CandleType.Minute)
                backTrackMinutes = -1;
            else if (candleType == CandleType.ThreeMinute)
                backTrackMinutes = -3;
            else if (candleType == CandleType.FiveMinute)
                backTrackMinutes = -5;
            return backTrackMinutes;
        }
    }
}
