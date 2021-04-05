using System;
using System.Collections.Generic;
using Orca.Tools;
using Orca.Entities;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace OrcaTests.Tools
{
    public class MockGraphHelper : IGraphHelper
    {
        private List<Subscription> _orcaSubscriptions;
        public MockGraphHelper()
        {
            _orcaSubscriptions = new List<Subscription>();
        }

        public async Task<Microsoft.Graph.User> GetUserAsync(string userId)
        {
            var user = new Microsoft.Graph.User()
            {
                Mail = "john.doe.20@ucl.ac.uk",
                GivenName = "John",
                Surname = "Doe",
                DisplayName = "test student",
                Id = userId
            };
            return user;
        }

        public async Task<string> GetUserIdByMailAsync(string mail)
        {
            return "12345678";
        }

        public async Task<string> GetCallRecordSessions(string callId)
        {

            var sessionsList = new List<Orca.Entities.Session>();
            if(callId == "44444")
            {
                sessionsList.Add(
                //This adds a participant that joined the call. If you want more copy paste it and change the user
                    new Orca.Entities.Session()
                    {
                        StartDateTime = DateTime.Now,
                        Caller = new Caller()
                        {
                            Identity = new Orca.Entities.Identity()
                            {
                                User = new Orca.Entities.User()
                                {
                                    DisplayName = "John Doe",
                                    Id = "12345678"
                                }
                            }
                        }
                    });
            }
            else
            {
                sessionsList.Add(
                //This adds a participant that joined the call. If you want more copy paste it and change the user
                    new Orca.Entities.Session()
                    {
                        StartDateTime = DateTime.Now,
                        Caller = new Caller()
                        {
                            Identity = new Orca.Entities.Identity()
                            {
                                User = new Orca.Entities.User()
                                {
                                    Id = "00000",
                                    DisplayName = "test student"
                                }
                            }
                        }
                    });
            }

            if (callId != null)
            {
                var callRecord = new CallRecord()
                {
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddHours(1),
                    JoinWebUrl = null,

                    Sessions = sessionsList,
                    Organizer = new Organizer()
                    {
                        User = new Orca.Entities.User()
                        {
                            DisplayName = "John Doe",
                            Id = "12345678"
                        }
                    },
                };
                if (callId == "22222")
                {
                    callRecord.JoinWebUrl = "notexistjoinweburl.com";
                }
                if (callId == "33333")
                {
                    callRecord.JoinWebUrl = "testjoinweburl.com";
                }
                var json = JsonConvert.SerializeObject(callRecord);
                return json;

            }

            return JsonConvert.SerializeObject(null);
        }

        public async Task<Subscription> CreateSubscription (int minutes)
        {
            var subscription = new Microsoft.Graph.Subscription()
            {
                NotificationUrl = "http://localhost/",
                Id = "7f105c7d-2dc5-4530-97cd-4e7ae6534c07",
                ApplicationId = "123456789abcdef",
                ChangeType = "created",
                ClientState = "xxxxxx",
                ExpirationDateTime = DateTime.Now.AddMinutes(minutes),
                CreatorId = "12345678",
                LatestSupportedTlsVersion = "v1_2",
                LifecycleNotificationUrl = "https://webhook.azurewebsites.net/api/send/lifecycleNotifications"
            };
            _orcaSubscriptions.Add(subscription);
            return subscription;

        }

        public void RenewSubscription(Subscription subscription, int minutes)
        {
            var ExpirationDateTime = DateTime.UtcNow.AddMinutes(minutes);
            _orcaSubscriptions.Remove(subscription);
            subscription.ExpirationDateTime = ExpirationDateTime;
            _orcaSubscriptions.Add(subscription);
        }

        public async void DeleteSubscription(String subscriptionId)
        {
            foreach (Subscription subscription in _orcaSubscriptions)
            {
                if (subscription.Id == subscriptionId)
                {
                    _orcaSubscriptions.Remove(subscription);
                    break;
                }
            }
        }

        public async Task<List<Subscription>> ListSubscriptions()
        {
            return _orcaSubscriptions;
        }

    }

}
