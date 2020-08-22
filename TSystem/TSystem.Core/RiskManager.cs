using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Contracts;
using TSystem.Core.Models;

namespace TSystem.Core
{
    public class RiskManager : IRiskManager
    {
        public static IRiskManager Instance { get; } = new RiskManager();
        private IList<IRule> Rules { get; set; } = new List<IRule>();
        public void Run(AnalysisModel model)
        {
            foreach(IRule rule in Rules)
            {
                rule.Check(model);
            }
        }
    }

    public interface IRule
    {
        bool Check(AnalysisModel model);
    }

    public abstract class Rule : IRule
    {
        public abstract bool Check(AnalysisModel model);
        public virtual void ComputePL()
        {
        }
    }
}
