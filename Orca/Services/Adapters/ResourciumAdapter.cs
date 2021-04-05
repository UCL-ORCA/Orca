using Orca.Entities;
using Orca.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services.Adapters
{
    /// <summary>
    /// Adapter to handle events from the Resourcium project managed by Louis De Wart (louis.wardt.19@ucl.ac.uk)
    /// </summary>
    public class ResourciumAdapter
    {
        public const string MANUAL_ATTENDANCE_COURSE_ID = "Resourcium";

        private readonly IEventAggregator _eventAggregator;

        public ResourciumAdapter(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task ProcessEvent(ResourciumEventDto manualAttendanceEvent)
        {
            StudentEvent convertedEvent = new StudentEvent
            {
                CourseID = MANUAL_ATTENDANCE_COURSE_ID,
                Student = new Student
                {
                    ID = manualAttendanceEvent.Student.ID,
                    Email = manualAttendanceEvent.Student.Email,
                    FirstName = manualAttendanceEvent.Student.FirstName,
                    LastName = manualAttendanceEvent.Student.LastName
                },
                Timestamp = manualAttendanceEvent.Timestamp,
                EventType = EventType.Attendance
            };
            await _eventAggregator.ProcessEvent(convertedEvent);
        }
    }

    public class ResourciumSettings
    {
        /// <summary>
        /// The secret key which must be included in requests from the Resourcium app
        /// </summary>
        public string ApiKey { get; set; }
    }
}
