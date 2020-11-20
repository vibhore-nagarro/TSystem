using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TSystem.Common.Enums;
using TSystem.Core;

namespace TSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        [HttpPost]
        [Route("marketmode")]
        public IActionResult ChangeMode(MarketEngineMode engineMode)
        {
            System.ChangeEngine(engineMode);
            return Ok();
        }

        [HttpGet]
        [Route("instruments")]
        public IActionResult GetInstrumentList()
        {
            var instruments = KiteEngine.Instance.Kite.GetInstruments();
            var fut = instruments.Where(r => r.InstrumentType.Contains("FUT")).ToList();

            AppData.Instance.Cache.CreateEntry("AllInstruments").Value = instruments;
            AppData.Instance.Cache.CreateEntry("FutInstruments").Value = fut;

            return Ok(instruments);
        }
    }
}
