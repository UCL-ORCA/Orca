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

namespace Orca.Scheduling
{
    /// <summary>
    /// A scheduler used for controlling checking of updated courses on Sharepoint through a background task
    /// </summary>
    public class CourseCatalogUpdater : BackgroundService
    {
        private readonly int _updateIntervalMs;
        private readonly ILogger<CourseCatalogUpdater> _logger;
        private SharepointCourseCatalog _catalog;

        public CourseCatalogUpdater(ILogger<CourseCatalogUpdater> logger, SharepointCourseCatalog catalog, IOptions<SharepointSettings> sharepointSettings)
        {
            _logger = logger;
            _catalog = catalog;
            _updateIntervalMs = 1000 * sharepointSettings.Value.CourseCatalogUpdateInterval;
        }
        // Executes the scheduler in the background. It uses a cancellation token to stop the scheduler at a fixed time period
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"CourseCatalogUpdater is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($" CourseCatalogUpdater background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Updating Course Catalog.");

                await UpdateCourseCatalog();

                await Task.Delay(_updateIntervalMs, stoppingToken);
            }

            _logger.LogInformation($"CourseCatalogUpdater background task is stopping.");
        }

        private async Task UpdateCourseCatalog()
        {
            await _catalog.UpdateInMemoryMapping();
        }

    }
}
