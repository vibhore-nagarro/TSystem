using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Entities
{
    public class Portfolio
    {
        public List<Position> Positions { get; set; } = new List<Position>();
        public List<Holding> Holdings { get; set; } = new List<Holding>();
    }
}
