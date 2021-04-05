using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orca.Entities.Dtos;
using Orca.Services.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Controllers
{
    [ApiController]
    [Route("api/events/caliper")]
    public class CaliperEventController : ControllerBase
    {
        public const string CALIPER_API_KEY_HEADER = "authorization";

        private readonly MoodleAdapter _moodleAdapter;
        private readonly string _caliperApiKey;

        public CaliperEventController(MoodleAdapter moodleAdapter, IOptions<CaliperSettings> caliperSettings)
        {
            _moodleAdapter = moodleAdapter;
            _caliperApiKey = caliperSettings.Value.ApiKey;
        }

        [HttpPost]
        public async Task<IActionResult> CaliperEvent([FromHeader(Name = CALIPER_API_KEY_HEADER)] string incomingApiKey, [FromBody] CaliperEventBatchDto caliperEventBatch)
        {
            if (incomingApiKey == null || incomingApiKey != _caliperApiKey)
            {
                return Unauthorized();
            }
            await _moodleAdapter.ProcessEvents(caliperEventBatch);
            return Ok();
        }
    }

    public class CaliperSettings
    {
        /// <summary>
        /// The Caliper API Key used by the Moodle server to send events to ORCA
        /// </summary>
        public string ApiKey { get; set; }
    }
}
