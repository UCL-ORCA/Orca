using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services
{
    public interface IIdentityResolver
    {
        /// <summary>
        /// Returns the ID of a user based on their email. If no user is found, returns null
        /// </summary>
        /// <param name="email">the email of the user we want to query</param>
        /// <returns>the id of the queried user or null if not found</returns>
        Task<string> GetUserIdByEmail(string email);
    }
}
