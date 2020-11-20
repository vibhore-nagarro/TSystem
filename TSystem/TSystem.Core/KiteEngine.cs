using KiteConnect;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Core
{
    public class KiteEngine
    {
        #region Data Members

        // Initialize key and secret of your app
        private string apiKey = "icgbzpgqrkyhjfq2";
        private string appSecret = "xo9ow8edaa3wgtfqu18tf2n5npnovjt6";
        private string userId = "ZW2177";

        private static readonly KiteEngine kiteEngine = new KiteEngine();

        #endregion

        private KiteEngine()
        {

        }

        #region Properties

        public static KiteEngine Instance
        {
            get { return kiteEngine; }
        }

        public Kite Kite { get; private set; }

        public Ticker Ticker { get; private set; }
        public string PublicToken { get; private set; }
        public string AccessToken { get; private set; }

        #endregion

        #region Methods
        private void InitSession()
        {
            AccessToken = "xB4kn7fcaLJLtx2u6EDMKtSzvg6XBK5Q";
            PublicToken = "o4gvpN7zm9EhrLHzvmnGCm4fpex7woA8";

            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(PublicToken))
            {
                string url = Kite.GetLoginURL();
                string requestToken = "2Cu2z51ZItZqip5VQIzVWNR013e6leev";
                User user = Kite.GenerateSession(requestToken, appSecret);

                AccessToken = user.AccessToken;
                PublicToken = user.PublicToken;
            }
        }
        public void Initialize()
        {
            Kite = new Kite(apiKey, Debug: false);
            Kite.SetSessionExpiryHook(OnTokenExpire);

            InitSession();

            Kite.SetAccessToken(AccessToken);
            Ticker = new Ticker(apiKey, AccessToken);
        }

        private void OnTokenExpire()
        {
        }

        #endregion
    }
}
