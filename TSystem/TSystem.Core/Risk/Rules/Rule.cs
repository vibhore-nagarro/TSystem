using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Models;

namespace TSystem.Core
{
    public interface IRule
    {
        bool Check(AnalysisModel model);
    }

    public abstract class Rule : IRule
    {
        public abstract bool Check(AnalysisModel model);
        public virtual void ComputePL(AnalysisModel model)
        {
            
        }
    }
}
