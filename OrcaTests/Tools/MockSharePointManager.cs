using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orca.Services;
using Orca.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace OrcaTests.Tools
{
    /// <summary>
    /// Dictionary-based implementation of the ISharepointManager used to help test classes which rely on Sharepoint
    /// </summary>
    public class MockSharepointManager : ISharepointManager
    {
        public readonly Dictionary<string, List<SharepointListItem>> mockEventList;
        
        public MockSharepointManager()
        {
            mockEventList = new Dictionary<string, List<SharepointListItem>>();
        }

        public void PrintItems()
        {
            foreach (var field in mockEventList)
            {
                foreach (var fields in mockEventList[field.Key])
                {
                    Console.WriteLine(fields);
                }
            }
        }
        public async Task<bool> AddItemToList(string listName, SharepointListItem item)
        {
            mockEventList[listName].Add(item);
            return true;
        }

        public async Task<List<SharepointListItem>> GetItemsFromList(string listName)
        {

            var itemsToReturn = new List<SharepointListItem>();
            for (var i = 0; i < mockEventList[listName].Count; ++i)
            {
                Dictionary<string, object> mockDictionary = new Dictionary<string, object>();
                foreach (var field in mockEventList[listName][i])
                {
                    mockDictionary.Add(field.Key, field.Value);
                }
                itemsToReturn.Add(new SharepointListItem(mockDictionary));

            }
            return itemsToReturn;
        }

        public bool CheckListExists(string listName)
        {
            return mockEventList.ContainsKey(listName);
        }

        /// <summary>
        /// Creates a new Sharepoint List. The description and fieldsAsXml params can be anything as they aren't used in
        /// this mock
        /// </summary>
        /// <param name="listName">Name of the list to create</param>
        /// <param name="description">Not used, can be anything</param>
        /// <param name="fieldsAsXml">Not used, can be anything</param>
        public void CreateList(string listName, string description, List<string> fieldsAsXml)
        {
            mockEventList.Add(listName, new List<SharepointListItem>());
        }

        public void Dispose()
        {
        }
    }

    public class MockSharepointCourseCatalog : ICourseCatalog
    {
        public readonly Dictionary<string, CourseCatalogType > mockCatalog;

        public MockSharepointCourseCatalog()
        {
            mockCatalog = new Dictionary<string, CourseCatalogType>();
        }

        public string GetListNameForCourse(string courseId)
        {
                return mockCatalog[courseId].ListName;
        }
        public bool CheckCourseIdExist(string courseId)
        {
            return mockCatalog.ContainsKey(courseId);
        }

        public async Task UpdateInMemoryMapping()
        {
            return;
        }

        public string GetCourseIDForJoinWebURL(string webURL)
        {
            return mockCatalog.FirstOrDefault(x => x.Value.JoinWebUrl == webURL).Key;
        }

        public bool CheckJoinWebURLExist(string webURL)
        {
            return (GetCourseIDForJoinWebURL(webURL) != null) ? true : false;
        }
    }

    public class CourseCatalogType
    {
        public string ListName { get; set; }
        public string JoinWebUrl { get; set; }
    }
}
