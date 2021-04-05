using Microsoft.Graph;
using Microsoft.Graph.CallRecords;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orca.Tools
{
    public interface IGraphHelper
    {
        Task<User> GetUserAsync(string userId);
        Task<string> GetUserIdByMailAsync(string mail);
        Task<string> GetCallRecordSessions(string callId);
        Task<Subscription> CreateSubscription(int minutes);
        void RenewSubscription(Subscription subscription, int minutes);
        void DeleteSubscription(String subscriptionId);
        Task<List<Subscription>> ListSubscriptions();


    }
}
