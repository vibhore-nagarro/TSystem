using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TSystem.Entities;

namespace TSystem.UI.Win
{
    public class AnalysisMessageReceiver
    {
        public async Task Start()
        {
            var connection = new HubConnectionBuilder()
                                 .WithUrl("https://example.com/chathub", HttpTransportType.WebSockets | HttpTransportType.LongPolling)
                                 .Build();
            connection.On<Candle>("ReceiveCandle", OnCandle);
            connection.On<Candle>("ReceiveHeikinAshi", OnHeikinAshi);
            connection.On<Signal>("ReceiveSignal", OnSignal);

            await connection.StartAsync();
        }

        private void OnCandle(Candle candle)
        {

        }

        private void OnHeikinAshi(Candle candle)
        {

        }

        private void OnSignal(Signal signal)
        {

        }
    }
}
