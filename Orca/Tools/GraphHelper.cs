using Microsoft.Graph;
using Microsoft.Graph.CallRecords;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using System.Security;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Orca.Tools
{
    public class GraphHelper: IGraphHelper
    {
        private static GraphServiceClient _graphClient;
        private readonly string _appId;
        private readonly string _tenantId;
        private readonly string _notificationUrl;
        private ILogger<GraphHelper> _logger;

        public GraphHelper(IOptions<MSGraphSettings> msGraphSettings, ILogger<GraphHelper> logger)
        {
            var settingsVal = msGraphSettings.Value;
            _appId = settingsVal.AppId;
            _tenantId = settingsVal.TenantId;
            _notificationUrl = settingsVal.Domain;
            string _clientSecret = settingsVal.ClientSecret;
            _logger = logger;

            // Initialize the auth provider with values from appsettings.json
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(_appId)
                .WithTenantId(_tenantId)
                .WithClientSecret(_clientSecret)
                .Build();

            //Install-Package Microsoft.Graph.Auth -PreRelease
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            _graphClient = new GraphServiceClient(authProvider);
        }

        public async Task<User> GetUserAsync(string userId)
        {
            try
            {
                // GET /users/{id}
                return await _graphClient.Users[userId]
                    .Request()
                    .Select(e => new
                    {
                        e.Mail,
                        e.GivenName,
                        e.Surname,
                        e.Id
                    })
                    .GetAsync();
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetUserIdByMailAsync(string mail)
        {
            try
            {
                // GET /users/{id}
                var result =  await _graphClient.Users
                    .Request()
                    .Select(e => new
                    {
                        e.Mail,
                        e.GivenName,
                        e.Surname,
                        e.Id
                    })
                    .Filter("mail eq '" + mail + "'")
                    .GetAsync();
                
                return result.Count == 0 ? null : result[0].Id;
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error getting users: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetCallRecordSessions(string callId)
        {
            try
            {
                // GET /groups/{groupId}/Members
                var result = await _graphClient.Communications
                    .CallRecords[callId]
                    .Request()
                    .Expand("sessions")
                    .GetAsync();
                var json = JsonConvert.SerializeObject(result);
                return json;
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error getting call record: {ex.Message}");
                return null;
            }
        }

        public async Task<Subscription> CreateSubscription(int minutes)
        {
            var sub = new Microsoft.Graph.Subscription();
            sub.ChangeType = "updated";
            sub.NotificationUrl = _notificationUrl + "/api/notifications";
            sub.Resource = "/communications/callRecords";
            sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(minutes);
            sub.ClientState = "SecretClientState";

            try
            {
                return await _graphClient
                    .Subscriptions
                    .Request()
                    .AddAsync(sub);
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error creating subscription: {ex.Message}");
                return null;
            }
        }

        public async void RenewSubscription(Subscription subscription, int minutes)
        {
            Console.WriteLine($"Current subscription: {subscription.Id}, Expiration: {subscription.ExpirationDateTime}");

            var newSubscription = new Subscription
            {
                ExpirationDateTime = DateTime.UtcNow.AddMinutes(minutes)
            };

            try
            {
                await _graphClient
                    .Subscriptions[subscription.Id]
                    .Request()
                    .UpdateAsync(newSubscription);

                subscription.ExpirationDateTime = newSubscription.ExpirationDateTime;
                _logger.LogInformation($"Renewed subscription: {subscription.Id}, New Expiration: {subscription.ExpirationDateTime}");
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error renewing subscription: {ex.Message}");
            }
        }

        public async void DeleteSubscription(String subscriptionId)
        {
            try
            {
                await _graphClient.Subscriptions[subscriptionId]
                    .Request()
                    .DeleteAsync();
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error deleting subscription: {ex.Message}");
            }
        }

        public async Task<List<Subscription>> ListSubscriptions()
        {
            try
            {
                var subscriptions = await _graphClient.Subscriptions
                    .Request()
                    .GetAsync();

                var subscriptionsList = new List<Subscription>();
                subscriptionsList.AddRange(subscriptions);
                
                subscriptionsList.RemoveAll(subscription => (subscription.Resource != "/communications/callRecords") || (subscription.NotificationUrl != (_notificationUrl + "/api/notifications")));
                return subscriptionsList;
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Error listing subscriptions: {ex.Message}");
                return null;
            }

        }
    }
}
