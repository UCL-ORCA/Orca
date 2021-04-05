using Microsoft.Extensions.Logging;
using Microsoft.Graph.CallRecords;
using Newtonsoft.Json;
using Orca.Entities;
using Orca.Entities.Dtos;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Session = Orca.Entities.Session;

namespace Orca.Services.Adapters
{
    public class MsGraphAdapter
    {
        private readonly IEventAggregator _eventAggregator;
        private IGraphHelper _graphHelper;
        private ILogger<MsGraphAdapter> _logger;
        private ICourseCatalog _courseCatalog;

        public MsGraphAdapter(IEventAggregator eventAggregator, ILogger<MsGraphAdapter> logger, IGraphHelper graphHelper, ICourseCatalog courseCatalog)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _graphHelper = graphHelper;
            _courseCatalog = courseCatalog;

        }

        public async Task ProcessEvents(string callRecordId)
        {

            var callRecordString = await _graphHelper.GetCallRecordSessions(callRecordId);
            var callRecord = JsonConvert.DeserializeObject<Entities.CallRecord>(callRecordString);

            var organizerId = (callRecord != null ? callRecord.Organizer.User.Id : null);
            var joinWebUrl = (callRecord != null ? callRecord.JoinWebUrl : null);
            if (joinWebUrl != null && _courseCatalog.CheckJoinWebURLExist(joinWebUrl))
            {
                await _courseCatalog.UpdateInMemoryMapping();
                string targetCourseID = _courseCatalog.GetCourseIDForJoinWebURL(joinWebUrl);
                foreach (Session session in callRecord.Sessions)
                {
                    var caller = session.Caller;
                    var user = await _graphHelper.GetUserAsync(caller.Identity.User.Id);

                    if (user != null && user.Id != organizerId)
                    {
                        StudentEvent studentEvent = new StudentEvent
                        {
                            CourseID = targetCourseID.ToUpper(),
                            Timestamp = ((DateTimeOffset)session.StartDateTime).UtcDateTime,
                            EventType = EventType.Attendance,
                            ActivityType = "Meeting",
                            ActivityName = "Weekly Lecture",
                            Student = new Student
                            {
                                Email = user.Mail,
                                FirstName = user.GivenName,
                                LastName = user.Surname,
                                ID = user.Id
                            }
                        };
                        _logger.LogDebug("Student to be processed: " + studentEvent.ToString());
                        await _eventAggregator.ProcessEvent(studentEvent);
                    }
                }
            }

        }

    }
}
