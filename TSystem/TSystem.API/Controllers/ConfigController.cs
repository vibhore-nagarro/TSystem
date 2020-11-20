using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TSystem.Common.Enums;

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
    }
}
