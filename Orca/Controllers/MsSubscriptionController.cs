using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Orca.Services;
using Orca.Services.MSGraphSubscription;
using Orca.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orca.Services.Adapters;
using Microsoft.Graph.CallRecords;
using Orca.Entities;
using EventType = Orca.Entities.EventType;

namespace Orca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly MSGraphSettings _config;
        private static Dictionary<string, Subscription> _subscriptions = new Dictionary<string, Subscription>();
        private ILogger<GraphHelper> _logger;
        private readonly MsGraphAdapter _graphAdapter;

        public NotificationsController(IOptions<MSGraphSettings> msGraphSettings, ILogger<GraphHelper> logger, MsGraphAdapter msGraphAdapter)
        {
            this._config = msGraphSettings.Value;
            this._logger = logger;
            _graphAdapter = msGraphAdapter;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromQuery] string validationToken = null)
        {
            // handle validation
            if (!string.IsNullOrEmpty(validationToken))
            {
                _logger.LogDebug($"Received Token: '{validationToken}'");
                return Ok(validationToken);
            }

            // handle notifications
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string content = await reader.ReadToEndAsync();

                _logger.LogDebug("Received notification: " + content);

                var notifications = JsonConvert.DeserializeObject<Notifications>(content);

                foreach (var notification in notifications.Items)
                {
                    _logger.LogDebug($"Received notification: '{notification.Resource}', {notification.ResourceData?.Id}");
                    await _graphAdapter.ProcessEvents(notification.ResourceData?.Id);
                }
            }

            return Ok();
        }
    }
    }
