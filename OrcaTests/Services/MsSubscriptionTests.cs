using Xunit;
using Orca.Services.Adapters;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Services;
using OrcaTests.Tools;
using Orca.Entities;
using Orca.Entities.Dtos;
using Moq;
using Orca.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using OrcaTests.Services.Adapters;
using Orca.Scheduling;
using System.Threading;

namespace OrcaTests.Services
{
    public class MsSubscriptionTests
    {
        MockGraphHelper _graphHelper = new MockGraphHelper();
        readonly MSGraphSettings _config = new MSGraphSettings();

        //Checks that a subscription is created when CheckSubscriptionsAsync() is called in the MsGraphSubscriptionUpdater.
        [Fact]
        public async Task SubsciptionCreated()
        {
            var _msGraphSubscriptionUpdater = new MsGraphSubscriptionUpdater(Options.Create(_config), new InMemoryLogger<MsGraphSubscriptionUpdater>(), _graphHelper);
            await _msGraphSubscriptionUpdater.CheckSubscriptionsAsync();
            var subscriptions =  await _graphHelper.ListSubscriptions();

            Assert.Single(subscriptions);
            var subscription = subscriptions[0];
            Assert.Equal("7f105c7d-2dc5-4530-97cd-4e7ae6534c07", subscription.Id);
            Assert.Equal("123456789abcdef", subscription.ApplicationId);
            Assert.Equal("12345678", subscription.CreatorId);

        }
    }
}
