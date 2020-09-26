using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/690691/day?user_id=ZW2177&oi=1&from=2020-03-26&to=2020-09-26&ciqrandom=1601116242950", Method.GET, DataFormat.Json);
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
                OnCandleReceived(candles.Last(), CandleType.Day);
            }
                       
        }
    }
}
