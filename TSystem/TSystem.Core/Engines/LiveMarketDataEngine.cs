using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TSystem.Common;
using TSystem.Entities;

namespace TSystem.Core
{
    
    public class LiveMarketDataEngine : IMarketDataEngine
    {
        Dictionary<uint, List<KiteConnect.Tick>> ticks = new Dictionary<uint, List<KiteConnect.Tick>>();
        public const string Token = "enctoken UNt/QY6GDQGNiCrUgw1l9H2kxA6lZ6CQSpVIbKOmLAbtd5ifAEXRnMag0lr2Lf+5MaBRWO/0pnCM2C7Crkq7+E4mBHsRig==";
        public event CandleReceivedEventHandler CandleReceived;
        private void OnCandleReceived(Candle candle, CandleType type)
        {
            CandleReceived?.Invoke(this, new CandleReceivedArgs() { Candle = candle, Type = type });
        }

        Timer timer1 = new Timer(1 * 1000 * 60);
        Timer timer3 = new Timer(3 * 1000 * 60);
        Timer timer5 = new Timer(5 * 1000 * 60);
        Timer timer10 = new Timer(10 * 1000 * 60);
        Timer timer15 = new Timer(15 * 1000 * 60);
        Timer secondsTimer = new Timer(1000);

        public void Start()
        {
            KiteEngine.Instance.Initialize();
            TickEngine.Instance.Start();
            TickEngine.Instance.Ticker.OnTick += Ticker_OnTick;
            InitializeTimers();

            secondsTimer.Start();
        }

        private void Ticker_OnTick(KiteConnect.Tick tickData)
        {
            try
            {
                var today = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(DateTime.Today.Ticks, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                if (tickData.Timestamp.Value.Date == today.Date)
                {
                    if (ticks.Keys.Contains(tickData.InstrumentToken))
                    {
                        ticks[tickData.InstrumentToken].Add(tickData);
                    }
                    else
                    {
                        ticks.Add(tickData.InstrumentToken, new List<KiteConnect.Tick>() { tickData });
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log($"Error in Tick {ex}");
            }
        }

        private void InitializeTimers()
        {
            secondsTimer.Elapsed += SecondsTimer_Elapsed;
            timer1.Elapsed += Timer1_Elapsed;
            timer3.Elapsed += Timer3_Elapsed;
            timer5.Elapsed += Timer5_Elapsed;
        }        

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 0 && e.SignalTime.Minute % 5 == 0)
                //if (e.SignalTime.Second == 0)
            {
                //timer1.Start();
                //timer3.Start();
                timer5.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
                Logger.Log("Timer started");                
            }            
        }

        long lastVolume = -1;

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessTimerTick(e, CandleType.Minute);
        }        

        private void Timer3_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessTimerTick(e, CandleType.ThreeMinute);
        }
        private void Timer5_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessTimerTick(e, CandleType.FiveMinute);
        }

        private void ProcessTimerTick(ElapsedEventArgs e, CandleType candleType)
        {
            //Parallel.ForEach(ticks, (instrumentTicks) => 
            //{
            //    var allTicks = instrumentTicks.Value;
            //    int backTrackMinutes = GetMinutesToTrack(candleType);
            //    var signalTime = e.SignalTime;
            //    //var signalTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(e.SignalTime.Ticks, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            //    var lastTicks = allTicks.Where(tick => tick.Timestamp >= signalTime.AddMinutes(backTrackMinutes) && tick.Timestamp < signalTime);

            //    if (lastTicks.Any() == false)
            //        return;

            //    var open = lastTicks.First().LastPrice;
            //    var close = lastTicks.Last().LastPrice;
            //    var high = lastTicks.Max(tick => tick.LastPrice);
            //    var low = lastTicks.Min(tick => tick.LastPrice);
            //    var volume = lastTicks.Last().Volume;

            //    var candle = new Candle()
            //    {
            //        Open = open,
            //        Close = close,
            //        High = high,
            //        Low = low,
            //        Volume = volume,
            //        TimeStamp = signalTime.AddMinutes(backTrackMinutes),
            //        Instrument = lastTicks.Last().InstrumentToken,
            //    };

            //    ulong netVolume = volume;
            //    if (lastVolume > -1)
            //    {
            //        netVolume = (ulong)(volume - lastVolume);
            //    }
            //    candle.CandleVolume = netVolume;
            //    lastVolume = (long)volume;

            //    OnCandleReceived(candle, CandleType.Minute);
            //    Logger.Log(candle.ToString());
            //});
            try
            {
                foreach (var instrument in ticks.Keys)
                {
                    var allTicks = ticks[instrument];
                    int backTrackMinutes = GetMinutesToTrack(candleType);
                    //var signalTime = e.SignalTime;
                    var signalTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(e.SignalTime.Ticks, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    var lastTicks = allTicks.Where(tick => tick.Timestamp >= signalTime.AddMinutes(backTrackMinutes) && tick.Timestamp < signalTime);

                    if (lastTicks.Any() == false)
                        return;

                    var open = lastTicks.First().LastPrice;
                    var close = lastTicks.Last().LastPrice;
                    var high = lastTicks.Max(tick => tick.LastPrice);
                    var low = lastTicks.Min(tick => tick.LastPrice);
                    var volume = lastTicks.Last().Volume;

                    var candle = new Candle()
                    {
                        Open = open,
                        Close = close,
                        High = high,
                        Low = low,
                        Volume = volume,
                        TimeStamp = signalTime.AddMinutes(backTrackMinutes),
                        Instrument = lastTicks.Last().InstrumentToken,
                    };

                    ulong netVolume = volume;
                    if (lastVolume > -1)
                    {
                        netVolume = (ulong)(volume - lastVolume);
                    }
                    candle.CandleVolume = netVolume;
                    lastVolume = (long)volume;

                    OnCandleReceived(candle, CandleType.Minute);
                    Logger.Log(candle.ToString());
                }
            }catch(Exception ex)
            {
                Logger.Log($"Error in processing Tick {ex}");
            }
        }
        
        private static int GetMinutesToTrack(CandleType candleType)
        {
            int backTrackMinutes = -1;
            if (candleType == CandleType.Minute)
                backTrackMinutes = -1;
            else if (candleType == CandleType.ThreeMinute)
                backTrackMinutes = -3;
            else if (candleType == CandleType.FiveMinute)
                backTrackMinutes = -5;
            return backTrackMinutes;
        }
    }
}
