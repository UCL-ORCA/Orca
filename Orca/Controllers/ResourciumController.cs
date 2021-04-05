using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orca.Entities.Dtos;
using Orca.Services.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Controllers
{
    /// <summary>
    /// Controller to handle events from the Resourcium project managed by Louis De Wart (louis.wardt.19@ucl.ac.uk)
    /// </summary>
    [Route("api/events/attendance")]
    [ApiController]
    public class ResourciumController : ControllerBase
    {
        private readonly string _resourciumApiKey;
        private readonly ResourciumAdapter _resourciumAdapter;

        public ResourciumController(ResourciumAdapter resourciumAdapter, IOptions<ResourciumSettings> resourciumSettings)
        {
            _resourciumApiKey = resourciumSettings.Value.ApiKey;
            _resourciumAdapter = resourciumAdapter;
        }

        [HttpPost]
        public async Task<IActionResult> PostManualAttendanceEvent([FromQuery(Name = "apiKey")] string incomingApiKey, [FromBody] ResourciumEventDto manualAttendanceEvent)
        {
            if (incomingApiKey == null || incomingApiKey != _resourciumApiKey)
            {
                return Unauthorized();
            }
            await _resourciumAdapter.ProcessEvent(manualAttendanceEvent);
            return Ok();
        }
    }
}
