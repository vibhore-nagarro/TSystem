using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TSystem.Entities.Enums;

namespace TSystem.Entities
{
    public class Portfolio
    {
        public static Portfolio Instance { get; } = new Portfolio();
        public List<Position> Positions { get; set; } = new List<Position>();
        public List<Holding> Holdings { get; set; } = new List<Holding>();

        private decimal pnl = 0.0m;
        public decimal PnL
        {
            get
            {
                pnl = pnl + CurrentPnL;
                return pnl;
            }
        }

        public decimal CurrentPnL
        {
            get
            {
                decimal totalPL = 0m;
                foreach (var p in Positions)
                {
                    decimal pl = (p.PnL < -0.05m) ? -0.05m : p.PnL;
                    totalPL += pl;
                }
                return totalPL;
            }
        }

        public void AddOrUpdatePosition(decimal averagePrice, int quanity, TradeType tradeType)
        {
            quanity = tradeType == TradeType.Long ? quanity : (quanity * -1);
            if (Positions.Count == 0)
                Positions.Add(new Position() { Average = averagePrice, Quantity = quanity, TradeType = tradeType, LTP = averagePrice });
            else
            {
                var position = Positions.First();
                if(position.TradeType == tradeType)
                    position.Quantity += quanity;
                else
                {
                    Positions.Add(new Position() { Average = averagePrice, Quantity = quanity, TradeType = tradeType, LTP = averagePrice });
                }                    
            }
            Debug.WriteLine($"P/L - {PnL}");
        }
    }
}
