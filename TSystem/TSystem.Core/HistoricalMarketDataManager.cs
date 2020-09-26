using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core
{
    public class HistoricalMarketDataManager : IMarketDataManager
    {
        public const string Token = "enctoken UNt/QY6GDQGNiCrUgw1l9H2kxA6lZ6CQSpVIbKOmLAbtd5ifAEXRnMag0lr2Lf+5MaBRWO/0pnCM2C7Crkq7+E4mBHsRig==";
        public event CandleReceivedEventHandler CandleReceived;

        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        public void Start()
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();
            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/690691/day?user_id=ZW2177&oi=1&from=2018-10-28&to=2019-04-29&ciqrandom=1601103441117", Method.GET, DataFormat.Json);
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
                OnCandleReceived(candle, CandleType.Day);
            }
                       
        }
    }
}
