using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSystem.Entities;
using System.Timers;

namespace TSystem.Core
{
    public class BackTestHistoricalDataEngine : IMarketDataEngine
    {
        public const string Token = "enctoken puIpofbE/gTQ+Sko42nvcOvHlWGEf+jFWV+5Tl7lq8kS7dJbnSNbgHh/e1GGqCss/ZCfq0eF0osCsXwmWFyzNdfroJdhiA==";
        public event CandleReceivedEventHandler CandleReceived;

        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        public void Start()
        {
            BackTest();
        }

        private async void BackTest()
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();
            string startDate = "2020-12-10";
            string endDate = "2020-12-10";
            uint[] instruments = new uint[]
            {
                11984386, 11983362, 
            };

            foreach (uint instrument in instruments)
            {
                string uri = $"https://kite.zerodha.com/oms/instruments/historical/{instrument}/5minute?user_id=ZW2177&oi=1&from={startDate}&to={endDate}&ciqrandom=1606573753955";
                RestRequest request = new RestRequest(uri, Method.GET, DataFormat.Json);

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
                        CandleVolume = candle[5],
                        Instrument = instrument,
                    });
                    OnCandleReceived(candles.Last(), CandleType.FiveMinute);
                    await Task.Delay(100);
                }
            }
        }
    }
}
