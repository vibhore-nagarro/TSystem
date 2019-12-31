using KiteConnect;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Common
{
    public class KiteAPIManager
    {
        #region Data Members

        // Initialize key and secret of your app
        private string apiKey = "icgbzpgqrkyhjfq2";
        private string appSecret = "xo9ow8edaa3wgtfqu18tf2n5npnovjt6";
        private string userId = "ZW2177";

        #endregion

        #region Properties

        public Kite Kite { get; private set; }

        public Ticker Ticker { get; private set; }

        #endregion

        #region Methods

        public void Initialize(string accessToken)
        {
            Kite = new Kite(apiKey, Debug: false);
            Kite.SetSessionExpiryHook(OnTokenExpire);

            Kite.SetAccessToken(accessToken);
            Ticker = new Ticker(apiKey, accessToken);
        }

        private void OnTokenExpire()
        {
        }

        #endregion
    }
}
