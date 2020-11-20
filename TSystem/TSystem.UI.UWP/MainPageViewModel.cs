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
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Signal> Signals { get; set; } = new ObservableCollection<Signal>();
        public ObservableCollection<Candle> Candles { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<Candle> HeikinAshi { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<LogEntry> Logs { get; set; } = new ObservableCollection<LogEntry>();

        Timer timer1 = new Timer(1 * 1000 * 60);
        public async void OnLoad()
        {
            await Start();

            timer1.Elapsed += Timer1_Elapsed;
            timer1.Start();
        }

        private async void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            RestClient client = new RestClient(serverURL + @"api/");
            IRestRequest request = new RestRequest("home/logs", Method.GET);

            var logs = client.Execute<List<LogEntry>>(request).Data;
            foreach(var log in logs)
            {
                if(Logs.FirstOrDefault(l => l.Timestamp == log.Timestamp) == null)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Logs.Add(log);
                    });                    
                }
            }
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
