using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Orca.Services
{
    /// <summary>
    /// Stores various pieces of information needed to connect to Microsoft Graph 
    /// </summary>
    public class MSGraphSettings
    {
        /// 
        /// <summary>
        /// The App ID registered on Azure AD
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// The Tenant ID registered on Azure AD
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The ClientSecret registered on Azure AD
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// The public host URL through which this application is exposed (e.g. https://myorcadeployment.azurewebsites.net). Required for the MS Graph API to send us updates about new Teams meetings.
        /// </summary>
        public string Domain { get; set; }

    }
}
