using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSystem.Core;
using TSystem.Entities;

namespace TSystem.API
{
    public class System
    {
        public static AnalysisEngine analysisManager;
        static HubConnection connection;
        public async static void Start()
        {
            analysisManager = new AnalysisEngine();
            analysisManager.CandleRecieved += AnalysisManager_CandleRecieved;
            analysisManager.SignalRecieved += AnalysisManager_SignalRecieved;
            analysisManager.HeikinAshiRecieved += AnalysisManager_HeikinAshiRecieved;

            connection = new HubConnectionBuilder()
                                 .WithUrl("https://localhost:44340/analysis", HttpTransportType.WebSockets | HttpTransportType.LongPolling)
                                 .Build();

            await connection.StartAsync();

            analysisManager.Start();
        }

        private static async void AnalysisManager_SignalRecieved(object sender, Core.Events.SignalRecievedEventArgs e)
        {
            await SendSignal(e.Signal);
        }

        private static async void AnalysisManager_CandleRecieved(object sender, Core.Events.CandleRecievedEventArgs e)
        {
            await SendCandle(e.Candle);
        }

        private static async void AnalysisManager_HeikinAshiRecieved(object sender, Core.Events.CandleRecievedEventArgs e)
        {
            await SendHeikinAshi(e.Candle);
        }

        public static async Task SendCandle(Candle candle)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendCandle", candle);
            }
        }
        public static async Task SendHeikinAshi(Candle candle)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendHeikinAshi", candle);
            }
        }
        public static async Task SendSignal(Signal signal)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendSignal", signal);
            }
        }
    }
}
