using KiteConnect;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSystem.Entities.Enums;

namespace TSystem.UI.UWP
{
    public class Server
    {
        string serverURL = "https://localhost:44340/";
        private static readonly Server instance = new Server();
        public static Server Instance { get { return instance; } }
        
        public async Task UpdateConfig(MarketEngineMode engineMode)
        {
            RestClient client = new RestClient(serverURL);
            IRestRequest request = new RestRequest(@"api/config/marketmode", Method.POST);
            request.AddQueryParameter("engineMode", ((int)engineMode).ToString());

            var response = await client.ExecuteAsync(request);
        }

        public async Task<List<Instrument>> GetInstruments()
        {
            RestClient client = new RestClient(serverURL);
            IRestRequest request = new RestRequest(@"api/config/instruments", Method.GET);            

            var response = await client.ExecuteAsync<List<Instrument>>(request);
            return response.Data;
        }
    }
}
