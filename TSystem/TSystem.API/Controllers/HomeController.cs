using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TSystem.Common;

namespace TSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public String Get()
        {
            return "Hello from Analysis System";
        }

        [HttpGet]
        [Route("logs")]
        public List<LogEntry> GetLog()
        {
            if (Logger.Logs.Count > 0)
                return Logger.Logs;
            else
                return new List<LogEntry>();
        }
    }
}
