using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core.UnitTests
{
    [TestClass]
    public class AnalysisManagerUnitTests
    {
        [TestMethod]
        public void TestHeikinAshi()
        {
            BackTest();
        }

        public void BackTest()
        {
            Analyzer analyzer = new Analyzer(715267);
            LoadData(analyzer);
        }
        public void LiveTest()
        {
            Analyzer analyzer = new Analyzer(715267);
            LoadData(analyzer);
        }

        private void LoadData(Analyzer analyzer)
        {
            IEnumerable<string> quotes = File.ReadAllLines(@"D:\StockData\USD_INR Historical Data.csv").Skip(1).Reverse();
            foreach (string quote in quotes)
            {
                string[] fields = quote.Split(',');
                DateTime timeStamp = DateTime.Parse((fields[0] + ',' + fields[1]).Trim('\"'));
                decimal close = decimal.Parse(fields[2].Trim('\"'));
                decimal open = decimal.Parse(fields[3].Trim('\"'));
                decimal high = decimal.Parse(fields[4].Trim('\"'));
                decimal low = decimal.Parse(fields[5].Trim('\"'));

                Candle candle = new Candle()
                {
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    TimeStamp = timeStamp
                };
                analyzer.Model.LTP = close;
                candle.Index = (uint)analyzer.Model.Candles.Count;
                analyzer.Model.Candles.Add(candle);
                analyzer.BuildHekinAshiCandle();

                if (analyzer.Model.HeikinAshi.Count == 1)
                {
                    analyzer.Model.HeikinAshi[0].Open = 64.975m;
                    analyzer.Model.HeikinAshi[0].Close = 64.903m;
                    analyzer.Model.HeikinAshi[0].High = 65.062m;
                    analyzer.Model.HeikinAshi[0].Low = 64.820m;
                }

                analyzer.BackTest();
            }
            analyzer.DumpOutput();
            decimal pl = 0m;
            Signal lastSignal = null;
            foreach (var signal in analyzer.Model.Signals)
            {
                if (signal.Type == SignalType.ShortEntry && (lastSignal == null || lastSignal.Type == SignalType.LongEntry))
                {
                    pl -= signal.Price;
                }
                if (signal.Type == SignalType.LongEntry && (lastSignal == null || lastSignal.Type == SignalType.ShortEntry))
                {
                    pl += signal.Price;
                }
                lastSignal = signal;
            }
            if (lastSignal.Type == SignalType.ShortEntry)
            {
                pl += lastSignal.Price;
            }
            else
            {
                pl -= lastSignal.Price;
            }
        }
    }
}
