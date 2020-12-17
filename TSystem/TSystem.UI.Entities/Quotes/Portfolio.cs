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

        public decimal PnL { get; set; }
        public decimal LastTradePnL { get; set; }

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
                    int remainingQuantity = position.Quantity + quanity;
                    if(remainingQuantity == 0)
                    {
                        LastTradePnL = position.PnL < -0.05m ? -0.05m : position.PnL;
                        PnL += LastTradePnL;
                        Positions.Remove(position);
                    }
                    else
                    {
                        position.Quantity = remainingQuantity;
                        LastTradePnL = position.PnL < -0.05m ? -0.05m : position.PnL;
                        PnL += LastTradePnL;                        
                    }
                }                    
            }
        }
    }
}
