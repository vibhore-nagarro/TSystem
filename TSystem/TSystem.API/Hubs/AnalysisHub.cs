using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSystem.Common;
using TSystem.Entities;

namespace TSystem.API.Hubs
{
    public interface IAnalysisOperation
    {
        Task ReceiveSignal(Signal signal);
        Task ReceiveCandle(Candle candle);
        Task ReceiveHeikinAshi(Candle candle);
        Task ReceiveLog(LogEntry logEntry);
    }
    public class AnalysisHub :  Hub<IAnalysisOperation>
    {
        public async Task SendSignal(Signal signal)
        {
            await Clients.All.ReceiveSignal(signal);
        }

        public async Task SendCandle(Candle candle)
        {
            await Clients.All.ReceiveCandle(candle);
        }

        public async Task SendHeikinAshi(Candle candle)
        {
            await Clients.All.ReceiveHeikinAshi(candle);
        }

        public async Task SendLog(LogEntry logEntry)
        {
            await Clients.All.ReceiveLog(logEntry);
        }
    }
}
