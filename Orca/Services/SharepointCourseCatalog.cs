using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.Options;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Orca.Services
{
    public class SharepointCourseCatalog : ICourseCatalog
    {
        public const string COURSE_CATALOG_LIST_DESCRIPTION = "Course Catalog of Course IDs and their Sharepoint List Names.";
        public const string COURSE_ID_FIELD = "CourseId";
        public const string SHAREPOINT_LIST_NAME_FIELD = "SharepointListName";
        public const string JOIN_WEB_URL_FIELD = "JoinWebURL";
        public readonly string COURSE_ID_FIELD_XML_SCHEMA = $"<Field DisplayName='{COURSE_ID_FIELD}' Type='Text' Required='TRUE' />";
        public readonly string SHAREPOINT_LIST_NAME_FIELD_XML_SCHEMA = $"<Field DisplayName='{SHAREPOINT_LIST_NAME_FIELD}' Type='Text' Required='TRUE' />";
        public readonly string JOIN_WEB_URL_FIELD_XML_SCHEMA = $"<Field DisplayName='{JOIN_WEB_URL_FIELD}' Type='Text' Required='FALSE' />";

        private ILogger<SharepointCourseCatalog> _logger;

        private string _courseCatalogListName;
        private Dictionary<string, string> _courseIdToSharepointListNameMapping;
        private Dictionary<string, string> _joinWebURLToCourseIdMapping;
        private ISharepointManager _sharepointManager;

        /// <summary>
        /// Instantiates a CourseCatalog backed by a Sharepoint List. If the Sharepoint List does not exist, it will be created
        /// </summary>
        /// <param name="sharepointSettings">The settings used to access the Sharepoint List storing the catalog</param>
        /// <param name="createSharepointManagerFunc">A function which returns an ISharepointManager. This will be used whenever the catalog needs to access Sharepoint</param>
        public SharepointCourseCatalog(IOptions<SharepointSettings> sharepointSettings, ILogger<SharepointCourseCatalog> logger, ISharepointManager sharepointManager)
        {
            _logger = logger;
            var settingsValue = sharepointSettings.Value;
            _courseCatalogListName = settingsValue.CourseCatalogListName;

            _courseIdToSharepointListNameMapping = new Dictionary<string, string>();
            _joinWebURLToCourseIdMapping = new Dictionary<string, string>();
            _sharepointManager = sharepointManager;

            if (!sharepointManager.CheckListExists(_courseCatalogListName))
            {
                _logger.LogInformation($"The List '{_courseCatalogListName}' used to store the Course Catalog does not exist. Creating it now.");
                _sharepointManager.CreateList(_courseCatalogListName, COURSE_CATALOG_LIST_DESCRIPTION, new List<string> { COURSE_ID_FIELD_XML_SCHEMA, SHAREPOINT_LIST_NAME_FIELD_XML_SCHEMA, JOIN_WEB_URL_FIELD_XML_SCHEMA });
            }
        }


        /// <summary>
        /// Updates the in memory Course ID to Sharepoint List Name Mapping.
        /// This method is thread safe because it simply updates the reference to _courseIdToSharepointListNameMapping
        /// </summary>
        /// <returns></returns>
        public async Task UpdateInMemoryMapping()
        {
            var courseMapping = await _sharepointManager.GetItemsFromList(_courseCatalogListName);
            var updatedCourseMapping = new Dictionary<string, string>();
            var updatedJoinWebUrlMapping = new Dictionary<string, string>();
            foreach (var courseEntry in courseMapping)
            {
                updatedCourseMapping.Add((string)courseEntry[COURSE_ID_FIELD], (string)courseEntry[SHAREPOINT_LIST_NAME_FIELD]);
                if(courseEntry[JOIN_WEB_URL_FIELD] != null)
                {
                    updatedJoinWebUrlMapping.Add((string)courseEntry[JOIN_WEB_URL_FIELD], (string)courseEntry[COURSE_ID_FIELD]);
                }
            }
            _courseIdToSharepointListNameMapping = updatedCourseMapping;
            _joinWebURLToCourseIdMapping = updatedJoinWebUrlMapping;


        }

        public string GetListNameForCourse(string courseId)
        {
            // Return course list or throw KeyNotFoundException.
            return _courseIdToSharepointListNameMapping[courseId];
        }

        public bool CheckCourseIdExist(string courseId)
        {
            return _courseIdToSharepointListNameMapping.ContainsKey(courseId);
        }

        public string GetCourseIDForJoinWebURL(string webURL)
        {
            // Return course ID or throw KeyNotFoundException.
            return _joinWebURLToCourseIdMapping[webURL];
        }

        public bool CheckJoinWebURLExist(string webURL)
        {
            return _joinWebURLToCourseIdMapping.ContainsKey(webURL);
        }

    }
}
