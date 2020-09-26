using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSystem.Entities;

namespace TSystem.UI.UWP
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Signal> Signals { get; set; } = new ObservableCollection<Signal>();
        public ObservableCollection<Candle> Candles { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<Candle> HeikinAshi { get; set; } = new ObservableCollection<Candle>();



        public async void OnLoad()
        {
            await Start();
        }

        public async Task Start()
        {
            var connection = new HubConnectionBuilder()
                                 .WithUrl("https://localhost:44340/analysis", HttpTransportType.WebSockets | HttpTransportType.LongPolling)
                                 .Build();
            connection.On<Candle>("ReceiveCandle", OnCandle);
            connection.On<Candle>("ReceiveHeikinAshi", OnHeikinAshi);
            connection.On<Signal>("ReceiveSignal", OnSignal);

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
    }
}
