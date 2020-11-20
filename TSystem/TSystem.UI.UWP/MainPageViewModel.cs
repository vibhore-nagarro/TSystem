using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TSystem.Entities;
using TSystem.UI.Entities;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace TSystem.UI.UWP
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string serverURL = "https://tsystem-api.azurewebsites.net/";
        //string serverURL = "https://localhost:44340/analysis";
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Signal> Signals { get; set; } = new ObservableCollection<Signal>();
        public ObservableCollection<Candle> Candles { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<Candle> HeikinAshi { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<LogEntry> Logs { get; set; } = new ObservableCollection<LogEntry>();
       
        public async void OnLoad()
        {
            await Start();
        }

        public async Task Start()
        {
            var connection = new HubConnectionBuilder()
                                 .WithUrl(serverURL + "analysis", HttpTransportType.WebSockets | HttpTransportType.LongPolling)
                                 .Build();            
            connection.On<Candle>("ReceiveCandle", OnCandle);
            connection.On<Candle>("ReceiveHeikinAshi", OnHeikinAshi);
            connection.On<Signal>("ReceiveSignal", OnSignal);
            connection.On<LogEntry>("ReceiveLog", OnLog);            

            await connection.StartAsync();
        }

        private void OnCandle(Candle candle)
        {
            Candles.Add(candle);
        }

        private void OnHeikinAshi(Candle candle)
        {
            HeikinAshi.Add(candle);
        }

        private void OnSignal(Signal signal)
        {
            Signals.Add(signal);
        }

        private void OnLog(LogEntry logEntry)
        {
            Logs.Add(logEntry);
        }
    }
}
