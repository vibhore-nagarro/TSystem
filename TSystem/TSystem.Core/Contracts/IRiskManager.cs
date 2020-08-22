using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Models;

namespace TSystem.Core.Contracts
{
    public interface IRiskManager
    {
        void Run(AnalysisModel model);
    }
}
