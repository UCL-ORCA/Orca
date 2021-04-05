using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services
{
    public interface ICourseCatalog
    {
        /// <summary>
        /// Returns the Sharepoint List Name associated with the given course ID
        /// </summary>
        /// <param name="courseId"></param>
        /// <exception cref="KeyNotFoundException">If there is no mapping for the given courseId</exception>
        /// <returns>The name of the Sharepoint List associated with the courseId</returns>
        string GetListNameForCourse(string courseId);
        string GetCourseIDForJoinWebURL(string webURL);
        bool CheckCourseIdExist(string courseId);
        bool CheckJoinWebURLExist(string webURL);
        Task UpdateInMemoryMapping();
    }
}
