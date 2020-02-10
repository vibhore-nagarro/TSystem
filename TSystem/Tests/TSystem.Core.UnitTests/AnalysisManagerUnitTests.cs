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

        int count = 0;
        private void LoadData(Analyzer analyzer)
        {
            //IEnumerable<string> quotes = File.ReadAllLines(@"D:\StockData\USD_INR Historical Data.csv").Skip(1).Reverse();
            IEnumerable<string> quotes = File.ReadAllLines(@"D:\StockData\Nifty 50.csv").Skip(1).Reverse();
            foreach (string quote in quotes)
            {
                string[] fields = quote.Split(',');
                DateTime timeStamp = DateTime.ParseExact(fields[0], "MMM dd yyyy", null);                
                decimal close = decimal.Parse(fields[1].Trim('\"'));
                decimal open = decimal.Parse(fields[2].Trim('\"'));
                decimal high = decimal.Parse(fields[3].Trim('\"'));
                decimal low = decimal.Parse(fields[4].Trim('\"'));

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

                analyzer.BackTest();
            }
            analyzer.DumpOutput();            
        }
    }
}
