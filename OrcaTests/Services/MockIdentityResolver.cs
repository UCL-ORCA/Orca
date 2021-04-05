using Orca.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrcaTests.Services
{
    class MockIdentityResolver : IIdentityResolver
    {
        public async Task<string> GetUserIdByEmail(string email)
        {
            return "mockid-" + email;
        }
    }
}
