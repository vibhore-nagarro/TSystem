using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core.Events
{
    public delegate void SignalRecievedEventHandler(object sender, SignalRecievedEventArgs e);
    public class SignalRecievedEventArgs : EventArgs
    {
        public Signal Signal { get; set; }
        public SignalRecievedEventArgs(Signal signal)
        {
            Signal = signal;
        }
    }
}
