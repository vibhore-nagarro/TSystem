using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using TSystem.Core;
using TSystem.Core.Events;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.UI.Win
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Signal> Signals { get; set; } = new ObservableCollection<Signal>();
        public ObservableCollection<Candle> Candles { get; set; } = new ObservableCollection<Candle>();
        public ObservableCollection<Candle> HeikinAshi { get; set; } = new ObservableCollection<Candle>();

        AnalysisManager analysisManager = new AnalysisManager();
        public void OnLoad()
        {
        }
    }
}
