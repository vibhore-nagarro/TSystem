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
        public const string Token = "enctoken Zl392X9DgswS1AuX/noyXQuh37cHYiSBK1MZz8b2uYprb3RuA0YQELWz6qsYrr8HIzIVMXmahgpQizoh2BTxxMR/jnlRqQ==";
        public event CandleReceivedEventHandler CandleReceived;

        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        public void Start()
        {
            RestClient client = new RestClient();
            List<Candle> candles = new List<Candle>();        
            RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/256265/day?user_id=ZW2177&oi=1&from=2020-04-01&to=2020-10-02&ciqrandom=1601611937269", Method.GET, DataFormat.Json);
            //RestRequest request = new RestRequest("https://kite.zerodha.com/oms/instruments/historical/13014274/5minute?user_id=ZW2177&oi=1&from=2020-10-01&to=2020-10-01&ciqrandom=1601484955463", Method.GET, DataFormat.Json);
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
