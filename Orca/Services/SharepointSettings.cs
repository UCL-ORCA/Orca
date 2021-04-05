using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Orca.Services
{
    /// <summary>
    /// Stores various pieces of information needed to connect to Sharepoint 
    /// </summary>
    public class SharepointSettings
    {
        /// <summary>
        /// The Sharepoint Client ID through which we will authenticate
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The Sharepoint Client Secret through which we will authenticate
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The URL to the sharepoint site
        /// </summary>
        public string SharepointUrl { get; set; }

        /// <summary>
        /// The name of the Sharepoint List where the course id to list name mapping is stored
        /// </summary>
        public string CourseCatalogListName { get; set; }

        /// <summary>
        /// How often the CourseCatalog is updated (in seconds)
        /// </summary>
        public int CourseCatalogUpdateInterval { get; set; }
    }
}
