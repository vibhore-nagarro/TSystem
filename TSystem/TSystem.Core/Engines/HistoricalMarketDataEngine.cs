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
    public class HistoricalMarketDataEngine : IMarketDataEngine
    {
        public const string Token = "enctoken sa/bV6/h9P7rwWQfcwNNraf0and9ptpBe/Q/9P84tFudBG/CCZtRTeFhvSCwJajpann/Qr1JHc72mgiIhE0blpKjxNSEQw==";
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

            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/12084738/5minute?user_id=ZW2177&oi=1&from=2020-11-13&to=2020-11-13&ciqrandom=1605271700157", Method.GET, DataFormat.Json);
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
                });
                OnCandleReceived(candles.Last(), CandleType.Minute);
                await Task.Delay(100);
            }
        }
    }
}
