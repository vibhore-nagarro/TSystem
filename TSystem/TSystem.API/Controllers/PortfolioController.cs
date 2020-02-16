using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return new Holdings()
            {
                new Holding() { Tradingsymbol = "APOLLOTYRE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ASHOKLEY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ASIANPAINT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "AXISBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "BAJAJFINSV", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "BAJFINANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "GOLDBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFCBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFCLIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HINDUNILVR", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ICICIBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "INFY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ITC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "KOTAKBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "LT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "NIFTYBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "PERSISTENT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "RELIANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "SBILIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "SBIN", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "TCS", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            };
        }

        [HttpGet]
        [Route("{id}/holdings")]
        public Holdings GetHoldings(string id)
        {
            Holdings holdings = new Holdings()
            {
                new Holding() { Tradingsymbol = "APOLLOTYRE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ASHOKLEY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ASIANPAINT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "AXISBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "BAJAJFINSV", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "BAJFINANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "GOLDBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFCBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HDFCLIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "HINDUNILVR", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ICICIBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "INFY", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "ITC", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "KOTAKBANK", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "LT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "NIFTYBEES", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "PERSISTENT", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "RELIANCE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "SBILIFE", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "SBIN", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
                new Holding() { Tradingsymbol = "TCS", Quantity = 30, AveragePrice = 167.1m, LastPrice = 159.05m, Exchange = "NSE", ISIN = "", Product = "CNC", PnL = -241.5m },
            };

            int count = new Random().Next(holdings.Count - 1);
            if (id == "11") count = holdings.Count;
            return new Holdings(holdings.Take(count));
        }
    }
}