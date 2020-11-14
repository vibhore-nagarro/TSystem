using KiteConnect;
using System;
using System.Collections.Generic;
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
        private void SubscribeToInstruments()
        {
            //tokens = new UInt32[] { 12084738 }; // - Nifty
            //tokens = new UInt32[] { 57477639 }; // - Gold
            tokens = new UInt32[] { 780803 }; // - USDINR
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
            Console.WriteLine(tickData.LastPrice);
        }

        private void OnOrderUpdate(Order OrderData)
        {
            
        }

        #endregion



        #endregion
    }
}
