using KiteConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TSystem.Core
{
    public class TickEngine
    {
        #region Data Members

        private Ticker ticker;
        KiteEngine kiteEngine = KiteEngine.Instance;
        private static readonly TickEngine tickEngine = new TickEngine();

        #endregion

        #region Constructor

        private TickEngine()
        {

        }

        #endregion

        #region Properties

        public static TickEngine Instance
        {
            get { return tickEngine; }
        }

        public Ticker Ticker { get { return ticker; } }
        public uint[] Tokens { get { return tokens; } }

        #endregion

        #region Methods

        public void Start()
        {
            ticker = kiteEngine.Ticker;
            InitTicker();
        }

        private void InitTicker()
        {
            ticker.OnTick += OnTick;
            ticker.OnReconnect += OnReconnect;
            ticker.OnNoReconnect += OnNoReconnect;
            ticker.OnError += OnError;
            ticker.OnClose += OnClose;
            ticker.OnConnect += OnConnect;
            ticker.OnOrderUpdate += OnOrderUpdate;

            ticker.EnableReconnect(Interval: 5, Retries: 50);
            ticker.Connect();

            SubscribeToInstruments();
        }

        UInt32[] tokens = null;
        decimal p1, p2;
        private void SubscribeToInstruments()
        {
            //tokens = new UInt32[] { 12084738 }; // - Nifty
            tokens = new UInt32[] { 57477639, 57437447 }; // - Gold
            //tokens = new UInt32[] { 780803 }; // - USDINR
            //tokens = new UInt32[] { 12084482 }; // - Bank Nifty

            ticker.Subscribe(tokens);
            ticker.SetMode(tokens, Mode: Constants.MODE_FULL);
        }

        #region Kite Events

        private void OnConnect()
        {
        }

        private void OnClose()
        {
        }

        private void OnError(string Message)
        {
            //Console.WriteLine($"Error - {Message}");
        }

        private void OnNoReconnect()
        {
        }

        private void OnReconnect()
        {
        }

        
        private void OnTick(Tick tickData)
        {
        }

        private void OnOrderUpdate(Order OrderData)
        {
            
        }

        #endregion



        #endregion
    }
}
