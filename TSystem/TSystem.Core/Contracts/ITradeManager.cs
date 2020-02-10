using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Entities;

namespace TSystem.Core.Contracts
{
    public interface ITradeManager
    {
        void ProcessSignal(Signal signal, Analyzer analyzer);
    }
}
