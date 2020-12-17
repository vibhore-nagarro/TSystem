using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Core
{
    public class AppData
    {
        private static readonly AppData instance = new AppData();
        private AppData()
        {

        }
        public static AppData Instance { get { return instance; } }

        public MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
    }
}
