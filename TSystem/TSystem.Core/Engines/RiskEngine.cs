using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Core.Models;

namespace TSystem.Core
{
    public class RiskEngine : IRiskEngine
    {
        private RiskEngine()
        {
        }
        public static IRiskEngine Instance { get; } = new RiskEngine();
        private IList<IRule> Rules { get; set; } = new List<IRule>();
        public void Run(AnalysisModel model)
        {
            foreach(IRule rule in Rules)
            {
                rule.Check(model);
            }
        }
    }    
}
