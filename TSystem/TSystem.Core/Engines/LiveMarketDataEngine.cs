﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using TSystem.Entities;

namespace TSystem.Core
{
    
    public class LiveMarketDataEngine : IMarketDataEngine
    {
        List<KiteConnect.Tick> ticks = new List<KiteConnect.Tick>();
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
            ticks.Add(tickData);
            //Debug.WriteLine(tickData.LastPrice);
        }

        private void InitializeTimers()
        {
            secondsTimer.Elapsed += SecondsTimer_Elapsed;
            timer1.Elapsed += Timer1_Elapsed;
            timer3.Elapsed += Timer3_Elapsed;
        }        

        private void SecondsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (e.SignalTime.Second == 0)
            {
                timer1.Start();
                //timer3.Start();
                secondsTimer.Stop();
                secondsTimer.Dispose();
            }            
        }

        long lastVolume = -1;

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            var lastTicks = ticks.Where(tick => tick.Timestamp >= e.SignalTime.AddMinutes(-1) && tick.Timestamp < e.SignalTime);
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
                TimeStamp = e.SignalTime.AddMinutes(-1)
            };

            ulong netVolume = volume;
            if (lastVolume > -1)
            {
                netVolume = (ulong)(volume - lastVolume);
            }
            candle.CandleVolume = netVolume;
            lastVolume = (long)netVolume;

            OnCandleReceived(candle, CandleType.Minute);
        }

        private void Timer3_Elapsed(object sender, ElapsedEventArgs e)
        {
            var lastTicks = ticks.Where(tick => tick.Timestamp >= e.SignalTime.AddMinutes(-3) && tick.Timestamp < e.SignalTime);
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
                TimeStamp = e.SignalTime.AddMinutes(-3)
            };

            ulong netVolume = volume;
            if (lastVolume > -1)
            {
                netVolume = (ulong)(volume - lastVolume);
            }
            candle.CandleVolume = netVolume;
            lastVolume = (long)netVolume;

            OnCandleReceived(candle, CandleType.Minute);
        }
    }
}
