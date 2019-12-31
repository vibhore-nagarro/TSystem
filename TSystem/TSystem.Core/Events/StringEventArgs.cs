using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Core.Events
{
    public delegate void StringEventHandler(object sender, StringEventArgs e);
    public class StringEventArgs : EventArgs
    {
        public string Data { get; set; }
        public StringEventArgs(string data)
        {
            Data = data;
        }
    }
}
