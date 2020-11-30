using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TA = TicTacTec.TA.Library;
using TSystem.Entities;
using System.Diagnostics;
using TSystem.Entities.Enums;

namespace TSystem.Core.Models
{
    public class MACD
    {
        public decimal Value { get; set; }
        public decimal Signal { get; set; }
        public decimal Histogram { get; set; }
    }

    public class Stochastic
    {
        public decimal Value { get; set; }
        public decimal Signal { get; set; }
    }


    public class AnalysisModel
    {
        #region Fields

        private List<Tick> ticks = new List<Tick>();
        private List<Signal> signals = new List<Signal>();
        private List<Candle> candles = new List<Candle>();
        private List<Candle> heikinAshi = new List<Candle>();

        #endregion

        #region Properties

        public List<Tick> Ticks
        {
            get
            {
                return ticks;
            }
        }

        public List<Signal> Signals
        {
            get
            {
                return signals;
            }
        }

        public List<Candle> Candles
        {
            get
            {
                return candles;
            }
        }

        public List<Candle> HeikinAshi
        {
            get
            {
                return heikinAshi;
            }
        }

        public Candle LeadingHeikinAshi { get; set; }
        public Candle LeadingCandle { get; set; }
        public Candle CurrentHeikinAshi { get { return HeikinAshi.Last(); } }
        public Candle CurrentCandle { get { return Candles.Last(); } }
        public Candle PreviousHeikinAshi { get { return HeikinAshi.OrderByDescending(c => c.Index).Skip(1).First(); } }
        public Candle PreviousCandle { get { return Candles.OrderByDescending(c => c.Index).Skip(1).First(); } }

        
        public decimal ProfitLoss
        {
            get;
            set;
        }

        public decimal LTP { get; set; }
        public long TotalVolume { get; set; }
        public decimal LastTradePL { get; set; }
        public decimal PeriodPeak { get; set; }
        public decimal PeriodTrough { get; set; }
        public decimal PreviousLow { get; set; } = decimal.MaxValue;
        public decimal PreviousHigh { get; set; } = decimal.MinValue;

        #endregion

        #region Methods

        public List<decimal> AveragePrice()
        {
            return Candles.Select(candle => (candle.Open + candle.High + candle.Low + candle.Close) / 4).ToList();
        }

        public decimal AverageCandleBody => Candles.Average(c => c.Body) / 2;
        

        public decimal AverageCandleLength => Candles.Average(c => c.Length);
        public decimal AverageHeikinAshiBody => HeikinAshi.Average(c => c.Body);


        public decimal AverageHeikinAshiLength => HeikinAshi.Average(c => c.Length);


        public ulong AverageVolume => (ulong)Candles.Average(x => (decimal)x.Volume);

        public decimal AveragePriceMovePerCandle => Candles.Count == 1 ? Candles[0].Body : Candles.Skip(1).Select(c => c.Close - Candles[Candles.IndexOf(c) - 1].Close).Average();


        public ulong AverageVolumeForPeriod(int period)
        {
            return (ulong)Candles.Skip(Candles.Count - period).Take(period).Average(x => (decimal)x.Volume);
        }
        public Trend CandleTrend(int candleCount)
        {
            if (Candles.Count < candleCount)
                return Trend.Unknown;

            var candles = Candles.Skip(Candles.Count - candleCount);
            var upCandlesCount = candles.Count(c => c.IsGreen);
            var downCandlesCount = candles.Count(c => c.IsRed);

            var firstCandle = candles.First();
            var lastCandle = candles.Last();
            
            if(firstCandle.IsGreen)
            {
                if (lastCandle.Close > firstCandle.Close && upCandlesCount > downCandlesCount)
                    return Trend.Up;
                if (lastCandle.Close < firstCandle.Open && upCandlesCount < downCandlesCount)
                    return Trend.Down;
            }
            if (firstCandle.IsRed)
            {
                if (lastCandle.Close > firstCandle.Open && upCandlesCount > downCandlesCount)
                    return Trend.Up;
                if (lastCandle.Close < firstCandle.Close && upCandlesCount < downCandlesCount)
                    return Trend.Down;
            }

            return Trend.Sideways;
        }
        public List<decimal> SMA(int period)
        {
            int startIndex, count;
            double[] sma = new double[Candles.Count];
            
            TA.Core.Sma(0, Candles.Count - 1, Candles.Select(quote => quote.Close).Cast<float>().ToArray(), period, out startIndex, out count, sma);

            return sma.Select(x => (decimal)x).ToList();
        }

        public List<decimal> EMA(int period)
        {
            int startIndex, count;
            double[] ema = new double[Candles.Count];

            TA.Core.Ema(0, Candles.Count - 1, Candles.Select(quote => (float)quote.Close).ToArray(), period, out startIndex, out count, ema);

            return ema.Select(x => (decimal)x).Take(ema.Count() - period + 1).ToList();
        }

        public List<MACD> MACD(int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
        {
            int startIndex, count;
            double[] macd = new double[Candles.Count];
            double[] macdSignal = new double[Candles.Count];
            double[] macdHistogram = new double[Candles.Count];
            List<MACD> macdData = new List<MACD>();

            TA.Core.Macd(0, Candles.Count - 1, Candles.Select(quote => (double)quote.Close).ToArray(), fastPeriod, slowPeriod, signalPeriod, out startIndex, out count, macd, macdSignal, macdHistogram);

            for (int index = 0; index < macd.Length; ++index)
            {
                macdData.Add(new MACD()
                {
                    Value = (decimal)macd[index],
                    Signal = (decimal)macdSignal[index],
                    Histogram = (decimal)macdHistogram[index],
                });
            }
            macdData = macdData.Take(count).ToList();
            if (macdData.Count > 0)
            {
                
            }
            return macdData;
        }

        public decimal RSI(int period = 14)
        {
            int startIndex, count;
            double[] rsi = new double[Candles.Count];

            TA.Core.Rsi(0, Candles.Count - 1, Candles.Select(quote => (double)quote.Close).ToArray(), period, out startIndex, out count, rsi);

            return rsi.Select(x => (decimal)x).Where(x => x != 0).LastOrDefault();
        }

        public List<Stochastic> Stochastic(int periodK = 14, int periodD = 3, int periodDN = 3)
        {
            List<Stochastic> stochastic = new List<Stochastic>();

            //var Candles = GetData().ToList();
            int start, count;
            double[] slowK = new double[Candles.Count];
            double[] slowD = new double[Candles.Count];
            TA.Core.Stoch(0, Candles.Count - 1, Candles.Select(x => (double)x.High).ToArray(), Candles.Select(x => (double)x.Low).ToArray(), Candles.Select(x => (double)x.Close).ToArray(), periodK, periodD, TA.Core.MAType.Sma, periodDN, TA.Core.MAType.Sma, out start, out count, slowK, slowD);
            for (int i = 0; i < count; ++i)
            {
                stochastic.Add(new Stochastic() { Value = (decimal)slowD[i], Signal = (decimal)slowK[i] });
            }

            return stochastic.ToList();
        }

        public List<decimal> WilliamR(int period)
        {
            int startIndex, count;
            double[] willR = new double[Candles.Count];

            TA.Core.WillR(0, Candles.Count - 1, Candles.Select(quote => (double)quote.High).ToArray(), Candles.Select(quote => (double)quote.Low).ToArray(), Candles.Select(quote => (double)quote.Close).ToArray(), period, out startIndex, out count, willR);

            return willR.Select(x => (decimal)x).ToList();
        }

        //private List<Candle> GetData()
        //{
        //    return new List<Candle>()
        //    {
        //        new Candle() { Open = 1555.25m, High = 1565.55m, Low = 1548.19m, Close = 1562.5m},
        //        new Candle() { Open = 1562.5m, High = 1579.58m, Low = 1562.5m, Close = 1578.78m},
        //        new Candle() { Open = 1578.78m, High = 1583m, Low = 1575.8m, Close = 1578.79m},
        //        new Candle() { Open = 1578.93m, High = 1592.64m, Low = 1578.93m, Close = 1585.16m},
        //        new Candle() { Open = 1585.16m, High = 1585.78m, Low = 1577.56m, Close = 1582.24m},
        //        new Candle() { Open = 1582.34m, High = 1596.65m, Low = 1582.34m, Close = 1593.61m},
        //        new Candle() { Open = 1593.58m, High = 1597.57m, Low = 1586.5m, Close = 1597.57m},
        //        new Candle() { Open = 1597.55m, High = 1597.55m, Low = 1581.28m, Close = 1582.7m},
        //        new Candle() { Open = 1582.77m, High = 1598.6m, Low = 1582.77m, Close = 1597.59m},
        //        new Candle() { Open = 1597.6m, High = 1618.46m, Low = 1597.6m, Close = 1614.42m},
        //        new Candle() { Open = 1614.4m, High = 1619.77m, Low = 1614.21m, Close = 1617.5m},
        //        new Candle() { Open = 1617.55m, High = 1626.03m, Low = 1616.64m, Close = 1625.96m},
        //        new Candle() { Open = 1625.95m, High = 1632.78m, Low = 1622.7m, Close = 1632.69m},
        //        new Candle() { Open = 1632.69m, High = 1635.01m, Low = 1623.09m, Close = 1626.67m},
        //        new Candle() { Open = 1626.69m, High = 1633.7m, Low = 1623.71m, Close = 1633.7m},
        //        new Candle() { Open = 1632.1m, High = 1636m, Low = 1626.74m, Close = 1633.77m},
        //        new Candle() { Open = 1633.75m, High = 1651.1m, Low = 1633.75m, Close = 1650.34m},
        //        new Candle() { Open = 1649.13m, High = 1661.49m, Low = 1646.68m, Close = 1658.78m},
        //        new Candle() { Open = 1658.07m, High = 1660.51m, Low = 1648.6m, Close = 1650.47m},
        //        new Candle() { Open = 1652.45m, High = 1667.47m, Low = 1652.45m, Close = 1667.47m},
        //        new Candle() { Open = 1665.71m, High = 1672.84m, Low = 1663.52m, Close = 1666.29m},
        //        new Candle() { Open = 1666.2m, High = 1674.93m, Low = 1662.67m, Close = 1669.16m},
        //        new Candle() { Open = 1669.39m, High = 1687.18m, Low = 1648.86m, Close = 1655.35m},
        //        new Candle() { Open = 1651.62m, High = 1655.5m, Low = 1635.53m, Close = 1650.51m},
        //        new Candle() { Open = 1646.67m, High = 1649.78m, Low = 1636.88m, Close = 1649.6m},
        //        new Candle() { Open = 1652.63m, High = 1674.21m, Low = 1652.63m, Close = 1660.06m},
        //        new Candle() { Open = 1656.57m, High = 1656.57m, Low = 1640.05m, Close = 1648.36m},
        //        new Candle() { Open = 1649.14m, High = 1661.91m, Low = 1648.61m, Close = 1654.41m},
        //        new Candle() { Open = 1652.13m, High = 1658.99m, Low = 1630.74m, Close = 1630.74m},
        //        new Candle() { Open = 1631.71m, High = 1640.42m, Low = 1622.72m, Close = 1640.42m},
        //        new Candle() { Open = 1640.73m, High = 1646.53m, Low = 1623.62m, Close = 1631.38m},
        //        new Candle() { Open = 1629.05m, High = 1629.31m, Low = 1607.09m, Close = 1608.9m},
        //        new Candle() { Open = 1609.29m, High = 1622.56m, Low = 1598.23m, Close = 1622.56m},
        //        new Candle() { Open = 1625.27m, High = 1644.4m, Low = 1625.27m, Close = 1643.38m},
        //        new Candle() { Open = 1644.67m, High = 1648.69m, Low = 1639.26m, Close = 1642.81m},
        //        new Candle() { Open = 1638.64m, High = 1640.13m, Low = 1622.92m, Close = 1626.13m},
        //        new Candle() { Open = 1629.94m, High = 1637.71m, Low = 1610.92m, Close = 1612.52m},
        //        new Candle() { Open = 1612.15m, High = 1639.25m, Low = 1608.07m, Close = 1636.36m},
        //        new Candle() { Open = 1635.52m, High = 1640.8m, Low = 1623.96m, Close = 1626.73m},
        //        new Candle() { Open = 1630.64m, High = 1646.5m, Low = 1630.34m, Close = 1639.04m},
        //        new Candle() { Open = 1639.77m, High = 1654.19m, Low = 1639.77m, Close = 1651.81m},
        //        new Candle() { Open = 1651.83m, High = 1652.45m, Low = 1628.91m, Close = 1628.93m},
        //        new Candle() { Open = 1624.62m, High = 1624.62m, Low = 1584.32m, Close = 1588.19m},
        //        new Candle() { Open = 1588.62m, High = 1599.19m, Low = 1577.7m, Close = 1592.43m},
        //        new Candle() { Open = 1588.77m, High = 1588.77m, Low = 1560.33m, Close = 1573.09m},
        //        new Candle() { Open = 1577.52m, High = 1593.79m, Low = 1577.09m, Close = 1588.03m},
        //        new Candle() { Open = 1592.27m, High = 1606.83m, Low = 1592.27m, Close = 1603.26m},
        //        new Candle() { Open = 1606.44m, High = 1620.07m, Low = 1606.44m, Close = 1613.2m},
        //        new Candle() { Open = 1611.12m, High = 1615.94m, Low = 1601.06m, Close = 1606.28m},
        //        new Candle() { Open = 1609.78m, High = 1626.61m, Low = 1609.78m, Close = 1614.96m},
        //        new Candle() { Open = 1614.29m, High = 1624.26m, Low = 1606.77m, Close = 1614.08m},
        //        new Candle() { Open = 1611.48m, High = 1618.97m, Low = 1604.57m, Close = 1615.41m},
        //        new Candle() { Open = 1618.65m, High = 1632.07m, Low = 1614.71m, Close = 1631.89m},
        //        new Candle() { Open = 1634.2m, High = 1644.68m, Low = 1634.2m, Close = 1640.46m},
        //        new Candle() { Open = 1642.89m, High = 1654.18m, Low = 1642.89m, Close = 1652.32m},
        //        new Candle() { Open = 1651.56m, High = 1657.92m, Low = 1647.66m, Close = 1652.62m},
        //        new Candle() { Open = 1657.41m, High = 1676.63m, Low = 1657.41m, Close = 1675.02m},
        //        new Candle() { Open = 1675.26m, High = 1680.19m, Low = 1672.33m, Close = 1680.19m},
        //        new Candle() { Open = 1679.59m, High = 1684.51m, Low = 1677.89m, Close = 1682.5m},
        //        new Candle() { Open = 1682.7m, High = 1683.73m, Low = 1671.84m, Close = 1676.26m},
        //        new Candle() { Open = 1677.91m, High = 1684.75m, Low = 1677.91m, Close = 1680.91m},
        //        new Candle() { Open = 1681.05m, High = 1693.12m, Low = 1681.05m, Close = 1689.37m},
        //        new Candle() { Open = 1686.15m, High = 1692.09m, Low = 1684.08m, Close = 1692.09m},
        //        new Candle() { Open = 1694.41m, High = 1697.61m, Low = 1690.67m, Close = 1695.53m},
        //        new Candle() { Open = 1696.63m, High = 1698.78m, Low = 1691.13m, Close = 1692.39m},
        //        new Candle() { Open = 1696.06m, High = 1698.38m, Low = 1682.57m, Close = 1685.94m},
        //        new Candle() { Open = 1685.21m, High = 1690.94m, Low = 1680.07m, Close = 1690.25m},
        //        new Candle() { Open = 1687.31m, High = 1691.85m, Low = 1676.03m, Close = 1691.65m},
        //        new Candle() { Open = 1690.32m, High = 1690.92m, Low = 1681.86m, Close = 1685.33m},
        //        new Candle() { Open = 1687.92m, High = 1693.19m, Low = 1682.42m, Close = 1685.96m},
        //        new Candle() { Open = 1687.76m, High = 1698.43m, Low = 1684.94m, Close = 1685.73m},
        //        new Candle() { Open = 1689.42m, High = 1707.85m, Low = 1689.42m, Close = 1706.87m},
        //        new Candle() { Open = 1706.1m, High = 1709.67m, Low = 1700.68m, Close = 1709.67m},
        //        new Candle() { Open = 1708.01m, High = 1709.24m, Low = 1703.55m, Close = 1707.14m},
        //        new Candle() { Open = 1705.79m, High = 1705.79m, Low = 1693.29m, Close = 1697.37m},
        //        new Candle() { Open = 1695.3m, High = 1695.3m, Low = 1684.91m, Close = 1690.91m},
        //        new Candle() { Open = 1693.35m, High = 1700.18m, Low = 1688.38m, Close = 1697.48m},
        //        new Candle() { Open = 1696.1m, High = 1699.42m, Low = 1686.02m, Close = 1691.42m},
        //        new Candle() { Open = 1688.37m, High = 1691.49m, Low = 1683.35m, Close = 1689.47m},
        //        new Candle() { Open = 1690.65m, High = 1696.81m, Low = 1682.62m, Close = 1694.16m},
        //        new Candle() { Open = 1693.88m, High = 1695.52m, Low = 1684.83m, Close = 1685.39m},
        //        new Candle() { Open = 1679.61m, High = 1679.61m, Low = 1658.59m, Close = 1661.32m},
        //        new Candle() { Open = 1661.22m, High = 1663.6m, Low = 1652.61m, Close = 1655.83m},
        //        new Candle() { Open = 1655.25m, High = 1659.18m, Low = 1645.84m, Close = 1646.06m},
        //        new Candle() { Open = 1646.81m, High = 1658.92m, Low = 1646.08m, Close = 1652.35m},
        //        new Candle() { Open = 1650.66m, High = 1656.99m, Low = 1639.43m, Close = 1642.8m},
        //        new Candle() { Open = 1645.03m, High = 1659.55m, Low = 1645.03m, Close = 1656.96m},
        //        new Candle() { Open = 1659.92m, High = 1664.85m, Low = 1654.81m, Close = 1663.5m},
        //        new Candle() { Open = 1664.29m, High = 1669.51m, Low = 1656.02m, Close = 1656.78m},
        //        new Candle() { Open = 1652.54m, High = 1652.54m, Low = 1629.05m, Close = 1630.48m},
        //        new Candle() { Open = 1630.25m, High = 1641.18m, Low = 1627.47m, Close = 1634.96m},
        //        new Candle() { Open = 1633.5m, High = 1646.41m, Low = 1630.88m, Close = 1638.17m},
        //        new Candle() { Open = 1638.89m, High = 1640.08m, Low = 1628.05m, Close = 1632.97m},
        //        new Candle() { Open = 1635.95m, High = 1651.35m, Low = 1633.41m, Close = 1639.77m},
        //        new Candle() { Open = 1640.72m, High = 1655.72m, Low = 1637.41m, Close = 1653.08m},
        //        new Candle() { Open = 1653.28m, High = 1659.17m, Low = 1653.07m, Close = 1655.08m},
        //        new Candle() { Open = 1657.44m, High = 1664.83m, Low = 1640.62m, Close = 1655.17m},
        //        new Candle() { Open = 1656.85m, High = 1672.4m, Low = 1656.85m, Close = 1671.71m},
        //        new Candle() { Open = 1675.11m, High = 1684.09m, Low = 1675.11m, Close = 1683.99m},
        //        new Candle() { Open = 1681.04m, High = 1689.13m, Low = 1678.7m, Close = 1689.13m},
        //        new Candle() { Open = 1689.21m, High = 1689.97m, Low = 1681.96m, Close = 1683.42m},
        //        new Candle() { Open = 1685.04m, High = 1688.73m, Low = 1682.22m, Close = 1687.99m},
        //        new Candle() { Open = 1691.7m, High = 1704.95m, Low = 1691.7m, Close = 1697.6m},
        //        new Candle() { Open = 1697.73m, High = 1705.52m, Low = 1697.73m, Close = 1704.76m},
        //        new Candle() { Open = 1705.74m, High = 1729.44m, Low = 1700.35m, Close = 1725.52m},
        //        new Candle() { Open = 1727.34m, High = 1729.86m, Low = 1720.2m, Close = 1722.34m},
        //        new Candle() { Open = 1722.44m, High = 1725.23m, Low = 1708.89m, Close = 1709.91m},
        //    };
        //}

        #endregion

    }
}
