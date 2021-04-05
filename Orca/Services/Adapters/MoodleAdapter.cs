using Microsoft.Extensions.Logging;
using Orca.Entities;
using Orca.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Services.Adapters
{
    public class MoodleAdapter
    {
        public const string COURSE_GROUP_TYPE = "http://purl.imsglobal.org/caliper/v1/lis/CourseSection";
        private readonly IIdentityResolver _identityResolver;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<MoodleAdapter> _logger;


        public MoodleAdapter(IEventAggregator eventAggregator, IIdentityResolver identityResolver, ILogger<MoodleAdapter> logger)
        {
            _identityResolver = identityResolver;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public async Task ProcessEvents(CaliperEventBatchDto caliperEventBatch)
        {
            var filteredCaliperEvents = caliperEventBatch.Data.Where(e => IsAboutStudent(e) && IsAboutCourse(e));
            foreach(var caliperEvent in filteredCaliperEvents)
            {
                string studentEmail = caliperEvent.Actor.Extensions.Email;
                string studentId = await _identityResolver.GetUserIdByEmail(studentEmail);
                if (studentId != null)
                {
                    //TODO cleaner way to get firstname/lastname
                    var studentName = caliperEvent.Actor.Name.Split(' ', 2);
                    string caliperActivityType = caliperEvent.Object.ObjectType;
                    string activityTypeWithoutUrl = caliperActivityType.Substring(caliperActivityType.LastIndexOf('/') + 1);
                    var studentEvent = new StudentEvent
                    {
                        CourseID = caliperEvent.Group.Name,
                        //TODO don't have this hardcoded in case of live lecture links
                        EventType = activityTypeWithoutUrl == "zoom" || activityTypeWithoutUrl == "teams" ? EventType.Attendance : EventType.Engagement,
                        ActivityName = caliperEvent.Object.Name,
                        ActivityType = activityTypeWithoutUrl,
                        Student = new Student { ID = studentId, FirstName = studentName[0], LastName = studentName[1], Email = studentEmail },
                        Timestamp = caliperEvent.EventTime
                    };
                    await _eventAggregator.ProcessEvent(studentEvent);
                } else
                {
                    _logger.LogError($"Event for student with email '{studentEmail}' ignored as no student ID could be resolved.");
                }
                
                
            }
        }

        private static bool IsAboutCourse(CaliperEventDto e)
        {
            return e.Group.GroupType == COURSE_GROUP_TYPE;
        }


        private static bool IsAboutStudent(CaliperEventDto e)
        {
            return e.Actor.ActorType == "http://purl.imsglobal.org/caliper/v1/lis/Person" && e.Membership.Roles.Any(role => role.Contains("Learner"));
        }


    }
}
