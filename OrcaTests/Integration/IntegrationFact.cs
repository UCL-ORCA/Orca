using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrcaTests.Integration
{
    /// <summary>
    /// Custom implementation of a Fact (Test attribute) which only runs if the ORCA_INTEGRATION env variable is set to TRUE
    /// </summary>
    class IntegrationFact : FactAttribute
    {
        public IntegrationFact()
        {
            string integrationTestsEnabled = Environment.GetEnvironmentVariable("ORCA_INTEGRATION");
            if (integrationTestsEnabled?.ToUpper() != "TRUE")
            {
                Skip = "Integration tests are ignored unless ORCA_INTEGRATION is set to TRUE";
            }
        }
    }
}
