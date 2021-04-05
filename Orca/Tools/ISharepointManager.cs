using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Tools
{
    /// <summary>
    /// Interface to manage interactions with Sharepoint
    /// </summary>
    public interface ISharepointManager : IDisposable
    {
        Task<bool> AddItemToList(string listName, SharepointListItem item);
        Task<List<SharepointListItem>> GetItemsFromList(string listName);

        bool CheckListExists(string listName);

        /// <summary>
        /// Creates a new sharepoint list
        /// </summary>
        /// <param name="listName">The name of the list</param>
        /// <param name="description">The description given to the list</param>
        /// <param name="fieldsAsXml">A list of XML schemas that define sharepoint list fields. See https://docs.microsoft.com/en-us/sharepoint/dev/sp-add-ins/complete-basic-operations-using-sharepoint-client-library-code#add-a-field-to-a-sharepoint-list</param>
        void CreateList(string listName, string description, List<string> fieldsAsXml);
    }
}

