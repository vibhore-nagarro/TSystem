using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSystem.Common;
using TSystem.Common.Enums;
using TSystem.Core;
using TSystem.Entities;

namespace TSystem.API
{
    public class System
    {
        private static AnalysisEngine analysisEngine;
        static HubConnection connection;
        static string serverURL = "https://tsystem-api.azurewebsites.net/analysis";
        //static string serverURL = "https://localhost:44340/analysis";

        public async static void Start()
        {
            analysisEngine = new AnalysisEngine();
            analysisEngine.CandleRecieved += AnalysisManager_CandleRecieved;
            analysisEngine.SignalRecieved += AnalysisManager_SignalRecieved;
            analysisEngine.HeikinAshiRecieved += AnalysisManager_HeikinAshiRecieved;
            Logger.OnLog += Logger_OnLog;

            connection = new HubConnectionBuilder()
                                 .WithUrl(serverURL, HttpTransportType.WebSockets | HttpTransportType.LongPolling)
                                 .Build();

            await connection.StartAsync();
            analysisEngine.Start();
        }

        public static void ChangeEngine(MarketEngineMode engineMode)
        {
            if (engineMode == MarketEngineMode.Live)
            {
                analysisEngine.ChangeEngine(new LiveMarketDataEngine());
                Logger.Log($"Market mode changed to Live");
            }
            else if(engineMode == MarketEngineMode.Historical)
            {
                analysisEngine.ChangeEngine(new BackTestHistoricalDataEngine());
                Logger.Log($"Market mode changed to Historical");
            }
        }

        #region Event Handlers

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

        private static async void Logger_OnLog(object sender, LogEventArgs args)
        {
            await SendLog(args.LogEntry);
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

        public static async Task SendLog(LogEntry logEntry)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendLog", logEntry);
            }
        }

        #endregion
    }
}
