using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TSystem.API.Models;

namespace TSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        public IEnumerable<PortfolioInfo> Get()
        {
            return new List<PortfolioInfo>()
            {
                new PortfolioInfo(){ Id = "11", Name = "All Investments" },
                new PortfolioInfo(){ Id = "1", Name = "Equity Portfolio" },
                new PortfolioInfo(){ Id = "2", Name = "Mutual Fund Portfolio" },
                new PortfolioInfo(){ Id = "3", Name = "ETF Portfolio" },
                new PortfolioInfo(){ Id = "4", Name = "Large Cap Portfolio" },
                new PortfolioInfo(){ Id = "5", Name = "Mid Cap Portfolio" },
                new PortfolioInfo(){ Id = "6", Name = "Small Cap Portfolio" },
                new PortfolioInfo(){ Id = "7", Name = "Banking Portfolio" },
                new PortfolioInfo(){ Id = "8", Name = "Technology Portfolio" },
                new PortfolioInfo(){ Id = "9", Name = "Cement Portfolio" },
                new PortfolioInfo(){ Id = "10", Name = "Pharma Portfolio" },
            };
        }

        [HttpGet]
        [Route("holdings")]
        public Holdings GetHoldings()
        {
            return new Holdings(GetMockHoldings());
        }

        [HttpGet]
        [Route("{id}/holdings")]
        public Holdings GetHoldings(string id)
        {
            List<Holding> holdings = GetMockHoldings();

            int count = new Random().Next(holdings.Count - 1);
            if (id == "11") count = holdings.Count;
            return new Holdings(holdings.Take(count));
        }

        private static List<Holding> GetMockHoldings()
        {
            //Holdings holdings = new Holdings()
            //{
            //    new Holding() { Tradingsymbol = "APOLLOTYRE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "ASHOKLEY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "ASIANPAINT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "AXISBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "BAJAJFINSV", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "BAJFINANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "GOLDBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "HDFC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "HDFCBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "HDFCLIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "HINDUNILVR", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "ICICIBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "INFY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "ITC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "KOTAKBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "LT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "NIFTYBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "PERSISTENT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "RELIANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "SBILIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "SBIN", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //    new Holding() { Tradingsymbol = "TCS", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            //};

            string json = @"[{'tradingsymbol':'APOLLOTYRE','exchange':'NSE','instrument_token':41729,'isin':'INE438A01022','product':'CNC','price':0,'quantity':30,'t1_quantity':0,'realised_quantity':30,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':167.1,'last_price':159.05,'close_price':159.05,'pnl':-241.4999999999995,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'ASHOKLEY','exchange':'NSE','instrument_token':54273,'isin':'INE208A01029','product':'CNC','price':0,'quantity':100,'t1_quantity':0,'realised_quantity':100,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':78.8,'last_price':80.25,'close_price':80.25,'pnl':145.00000000000028,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'ASIANPAINT','exchange':'NSE','instrument_token':60417,'isin':'INE021A01026','product':'CNC','price':0,'quantity':3,'t1_quantity':0,'realised_quantity':3,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':1700,'last_price':1877.05,'close_price':1877.2,'pnl':531.1499999999999,'day_change':-0.15000000000009095,'day_change_percentage':-0.007990624334119484},{'tradingsymbol':'AXISBANK','exchange':'NSE','instrument_token':1510401,'isin':'INE238A01034','product':'CNC','price':0,'quantity':10,'t1_quantity':0,'realised_quantity':10,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':709.67,'last_price':736.5,'close_price':736.5,'pnl':268.3000000000004,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'BAJAJFINSV','exchange':'NSE','instrument_token':4268801,'isin':'INE918I01018','product':'CNC','price':0,'quantity':1,'t1_quantity':0,'realised_quantity':1,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':9022,'last_price':9690.5,'close_price':9690.5,'pnl':668.5,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'BAJFINANCE','exchange':'NSE','instrument_token':81153,'isin':'INE296A01024','product':'CNC','price':0,'quantity':1,'t1_quantity':0,'realised_quantity':1,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':4308.6,'last_price':4781.75,'close_price':4783.25,'pnl':473.14999999999964,'day_change':-1.5,'day_change_percentage':-0.0313594313489782},{'tradingsymbol':'GOLDBEES','exchange':'NSE','instrument_token':3693569,'isin':'INF204KB17I5','product':'CNC','price':0,'quantity':500,'t1_quantity':0,'realised_quantity':500,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':33.5449,'last_price':35.85,'close_price':35.85,'pnl':1152.5500000000015,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'HDFC','exchange':'NSE','instrument_token':340481,'isin':'INE001A01036','product':'CNC','price':0,'quantity':4,'t1_quantity':0,'realised_quantity':4,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':2341.45,'last_price':2401.75,'close_price':2401.75,'pnl':241.20000000000073,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'HDFCBANK','exchange':'NSE','instrument_token':341249,'isin':'INE040A01034','product':'CNC','price':0,'quantity':8,'t1_quantity':0,'realised_quantity':8,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':1211.0625,'last_price':1219.35,'close_price':1219.35,'pnl':66.29999999999927,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'HDFCLIFE','exchange':'NSE','instrument_token':119553,'isin':'INE795G01014','product':'CNC','price':0,'quantity':10,'t1_quantity':0,'realised_quantity':10,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':568.9,'last_price':573.95,'close_price':573.95,'pnl':50.50000000000068,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'HINDUNILVR','exchange':'NSE','instrument_token':356865,'isin':'INE030A01027','product':'CNC','price':0,'quantity':6,'t1_quantity':0,'realised_quantity':6,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':2015.575,'last_price':2255.05,'close_price':2255.05,'pnl':1436.8500000000008,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'ICICIBANK','exchange':'NSE','instrument_token':1270529,'isin':'INE090A01021','product':'CNC','price':0,'quantity':15,'t1_quantity':0,'realised_quantity':15,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':520.883333,'last_price':545.8,'close_price':545.85,'pnl':373.7500049999994,'day_change':-0.05000000000006821,'day_change_percentage':-0.00916002564808431},{'tradingsymbol':'INFY','exchange':'NSE','instrument_token':408065,'isin':'INE009A01021','product':'CNC','price':0,'quantity':8,'t1_quantity':0,'realised_quantity':8,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':650.7,'last_price':786.45,'close_price':786.45,'pnl':1086,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'ITC','exchange':'NSE','instrument_token':424961,'isin':'INE154A01025','product':'CNC','price':0,'quantity':40,'t1_quantity':0,'realised_quantity':40,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':247.5,'last_price':207.7,'close_price':207.75,'pnl':-1592.0000000000005,'day_change':-0.05000000000001137,'day_change_percentage':-0.02406738868833279},{'tradingsymbol':'KOTAKBANK','exchange':'NSE','instrument_token':492033,'isin':'INE237A01028','product':'CNC','price':0,'quantity':3,'t1_quantity':0,'realised_quantity':3,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':1582.05,'last_price':1680.95,'close_price':1681.7,'pnl':296.7000000000003,'day_change':-0.75,'day_change_percentage':-0.04459772848902896},{'tradingsymbol':'LT','exchange':'NSE','instrument_token':2939649,'isin':'INE018A01030','product':'CNC','price':0,'quantity':6,'t1_quantity':0,'realised_quantity':6,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':1336.333333,'last_price':1295.15,'close_price':1295.15,'pnl':-247.0999979999997,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'NIFTYBEES','exchange':'NSE','instrument_token':2707457,'isin':'INF204KB14I2','product':'CNC','price':0,'quantity':410,'t1_quantity':0,'realised_quantity':410,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':122.827634,'last_price':128.38,'close_price':128.38,'pnl':2276.470059999997,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'PERSISTENT','exchange':'NSE','instrument_token':4701441,'isin':'INE262H01013','product':'CNC','price':0,'quantity':7,'t1_quantity':0,'realised_quantity':7,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':693.8,'last_price':701.25,'close_price':701.5,'pnl':52.15000000000032,'day_change':-0.25,'day_change_percentage':-0.03563791874554526},{'tradingsymbol':'RELIANCE','exchange':'NSE','instrument_token':738561,'isin':'INE002A01018','product':'CNC','price':0,'quantity':6,'t1_quantity':0,'realised_quantity':6,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':1408.35,'last_price':1487.6,'close_price':1487.6,'pnl':475.5,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'SBILIFE','exchange':'NSE','instrument_token':5582849,'isin':'INE123W01016','product':'CNC','price':0,'quantity':5,'t1_quantity':0,'realised_quantity':5,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':904.8,'last_price':914.25,'close_price':914.25,'pnl':47.25000000000023,'day_change':0,'day_change_percentage':0},{'tradingsymbol':'SBIN','exchange':'NSE','instrument_token':779521,'isin':'INE062A01020','product':'CNC','price':0,'quantity':16,'t1_quantity':0,'realised_quantity':16,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':301.8,'last_price':319.4,'close_price':319.55,'pnl':281.59999999999945,'day_change':-0.1500000000000341,'day_change_percentage':-0.04694101079644315},{'tradingsymbol':'TCS','exchange':'NSE','instrument_token':2953217,'isin':'INE467B01029','product':'CNC','price':0,'quantity':3,'t1_quantity':0,'realised_quantity':3,'collateral_quantity':0,'collateral_type':'','discrepancy':false,'average_price':2050,'last_price':2184.2,'close_price':2184.2,'pnl':402.59999999999945,'day_change':0,'day_change_percentage':0}]";
            List<Holding> holdings = JsonConvert.DeserializeObject<List<Holding>>(json);
            return holdings;
        }
    }
}