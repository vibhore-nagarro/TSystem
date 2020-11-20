using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TSystem.Entities;
using TSystem.Entities.Enums;
using TSystem.UI.Entities;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace TSystem.UI.UWP
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //string serverURL = "https://tsystem-api.azurewebsites.net/";
        string serverURL = "https://localhost:44340/";
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Signal> Signals { get; set; } = new ObservableCollection<Signal>();
        public ObservableCollection<Candle> Candles { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<Candle> HeikinAshi { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<LogEntry> Logs { get; set; } = new ObservableCollection<LogEntry>();
        public MarketEngineMode SelectedMarketEngineMode { get; set; } = MarketEngineMode.Live;
        public List<MarketEngineMode> MarketEngineModes { get; set; } = new List<MarketEngineMode>() { MarketEngineMode.Historical, MarketEngineMode.Live };

        public async void OnLoad()
        {
            await Start();
            //LoadInstruments();
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

        public async void UpdateConfig()
        {
            await Server.Instance.UpdateConfig(SelectedMarketEngineMode);
        }
        public async void LoadInstruments()
        {
            List<string> db = new List<string>();
            var response = await Server.Instance.GetInstruments();
            foreach (var item in response)
            {
                string line = $"{item.InstrumentToken},{item.Name},{item.TradingSymbol},{item.Exchange},{item.Segment},{item.InstrumentType},{item.Expiry},{item.LotSize},{item.Strike},{item.TickSize},{item.ExchangeToken}";
                db.Add(line);
            }

            var fut = response.Where(r => r.InstrumentType.Contains("FUT")).ToList();

            AppData.Instance.Cache.CreateEntry("Instruments").Value = fut;
            
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("masterDB.csv", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteLinesAsync(storageFile, db);
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
