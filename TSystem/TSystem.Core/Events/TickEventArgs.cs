using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core.Events
{
    public delegate void TickEventHandler(object sender, TickEventArgs e);
    public class TickEventArgs : EventArgs
    {
        public Tick Tick { get; set; }
        public TickEventArgs(Tick tick)
        {
            Tick = tick;
        }
    }
}
