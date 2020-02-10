using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TSystem.Core.Events;
using TSystem.Entities;
using TSystem.Entities.Enums;

namespace TSystem.Core
{
    class AnalysisManager
    {
        #region Data Members

        private ConcurrentDictionary<uint, Analyzer> analyzers = new ConcurrentDictionary<uint, Analyzer>();

        #endregion

        #region Methods

        internal void ProcessTick(Tick tickData)
        {
            Analyzer analyzer;
            if (analyzers.ContainsKey(tickData.InstrumentToken) == false)
            {
                analyzer = new Analyzer(tickData.InstrumentToken);
                analyzer.CandleRecieved += Analyzer_CandleRecievedEvent;
                analyzer.SignalRecieved += Analyzer_SignalRecievedEvent;

                analyzers.TryAdd(tickData.InstrumentToken, analyzer);
            }
            analyzer = analyzers[tickData.InstrumentToken];

            analyzer.Update(tickData);
        }

        public void BackTest()
        {
            
        }

        private void Analyzer_SignalRecievedEvent(object sender, SignalRecievedEventArgs e)
        {
        }

        private void Analyzer_CandleRecievedEvent(object sender, CandleRecievedEventArgs e)
        {
        }        

        #endregion
    }
}
