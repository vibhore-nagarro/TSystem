using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Models;
using TSystem.Entities;

namespace TSystem.Core.Contracts
{
    public interface IStrategy
    {
        Signal Apply(AnalysisModel model);
    }
}
