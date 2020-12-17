using System;
using System.Collections.Generic;
using System.Text;
using TSystem.Core.Models;

namespace TSystem.Core.Contracts
{
    public interface IRiskEngine
    {
        void Run(AnalysisModel model);
    }
}
