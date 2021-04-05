using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orca.Entities;
using Orca.Tools;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Orca.Scheduling
{
    /// <summary>
    /// A scheduler used for creating an MS Graph Webhook subscription for /communications/callRecord and updating it.
    /// </summary>
    public class MsGraphSubscriptionUpdater : BackgroundService
    {
        private const int _dELAY_TIME_MS = 5 * 60 * 1000;
        private const int _subscriptionMinutes = 15;
        private readonly ILogger<MsGraphSubscriptionUpdater> _logger;
        private IGraphHelper _graphHelper;
        private readonly MSGraphSettings _config;

        public MsGraphSubscriptionUpdater(IOptions<MSGraphSettings> msGraphSettings,ILogger<MsGraphSubscriptionUpdater> logger, IGraphHelper graphHelper)
        {
            this._config = msGraphSettings.Value;
            _logger = logger;
            _graphHelper = graphHelper;
        }
        // Executes the scheduler in the background. It uses a cancellation token to stop the scheduler at a fixed time period
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"MsGraphSubscriptionUpdater is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($" MsGraphSubscriptionUpdater background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Updating MsGraph subscription.");
                await CheckSubscriptionsAsync();

                await Task.Delay(_dELAY_TIME_MS, stoppingToken);
            }

            var subscriptions = await _graphHelper.ListSubscriptions();
            foreach (var subcription in subscriptions)
            {
                _graphHelper.DeleteSubscription(subcription.Id);
            }

            _logger.LogInformation($"MsGraphSubscriptionUpdater background task is stopping.");
        }

        public async Task CheckSubscriptionsAsync()
        {
            var subscriptions = await _graphHelper.ListSubscriptions();
            _logger.LogDebug($"Current subscription count {subscriptions.Count()}");

            if (subscriptions.Count() == 0)
            {
                _logger.LogDebug("Creating subscription...");
                var newSubscription = await _graphHelper.CreateSubscription(_subscriptionMinutes);
            }
            else
            {
                foreach (var subscription in subscriptions)
                {
                    // if the subscription expires in the next 5 min, renew it
                    if (subscription.ExpirationDateTime < DateTime.UtcNow.AddMinutes(5))
                    {
                        _graphHelper.RenewSubscription(subscription, _subscriptionMinutes);
                    }
                }
            }

        }

    }
}
