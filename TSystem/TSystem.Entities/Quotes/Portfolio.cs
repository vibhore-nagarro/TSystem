using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public class Portfolio
    {
        public static Portfolio Instance { get; } = new Portfolio();
        public List<Position> Positions { get; set; } = new List<Position>();
        public List<Holding> Holdings { get; set; } = new List<Holding>();
    }
}
