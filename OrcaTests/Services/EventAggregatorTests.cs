using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Entities;
using Orca.Services;
using OrcaTests.Tools;
namespace OrcaTests.Services
{
    public class EventAggregatorTests
    {
        [Fact]
        public async Task ProcessEventStoresAttendanceEventOnSharepoint()
        {
            var mockSharepointManager = new MockSharepointManager();
            var courseCatalog = new MockSharepointCourseCatalog();
            string courseId = "COMP0101";
            string listToStoreEvents = "Attendance Events";
            courseCatalog.mockCatalog.Add(courseId, new CourseCatalogType {ListName = listToStoreEvents, JoinWebUrl = null });
            mockSharepointManager.CreateList(listToStoreEvents, "", new List<string>());

            var eventAggregator = new EventAggregator(mockSharepointManager, courseCatalog, new InMemoryLogger<EventAggregator>());

            StudentEvent eventToStore = new StudentEvent
            {
                CourseID = courseId,
                EventType = EventType.Attendance,
                ActivityType = "Video",
                ActivityName = "Introductory Lesson",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            };
            await eventAggregator.ProcessEvent(eventToStore);

            var itemsInSharepointList = await mockSharepointManager.GetItemsFromList(listToStoreEvents);
            Assert.Single(itemsInSharepointList);
            var itemInList = itemsInSharepointList[0];
            Assert.Equal(eventToStore.CourseID, (string) itemInList["CourseId"]);
            Assert.Equal(eventToStore.Student.Email, (string) itemInList["StudentEmail"]);
            Assert.Equal(eventToStore.ActivityType, (string) itemInList["ActivityType"]);
            Assert.Equal(eventToStore.ActivityName, (string) itemInList["ActivityName"]);
            Assert.Equal(eventToStore.Timestamp, (DateTime) itemInList["Timestamp"]);
        }

        [Fact]
        public async Task ProcessEventIgnoresAttendanceEventIfNotFoundInCourseCatalog()
        {
            var mockSharepointManager = new MockSharepointManager();
            var emptyCourseCatalog = new MockSharepointCourseCatalog();
            const string listToStoreEvents = "Attendance Events";
            mockSharepointManager.CreateList(listToStoreEvents, "", new List<string>());
            const string courseIdNotInCatalog = "COMP0101";

            var eventAggregator = new EventAggregator(mockSharepointManager, emptyCourseCatalog, new InMemoryLogger<EventAggregator>());

            StudentEvent eventToStore = new StudentEvent
            {
                CourseID = courseIdNotInCatalog,
                EventType = EventType.Attendance,
                ActivityType = "Video",
                ActivityName = "Introductory Lesson",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            };
            await eventAggregator.ProcessEvent(eventToStore);

            var itemsInSharepointList = await mockSharepointManager.GetItemsFromList(listToStoreEvents);
            Assert.Empty(itemsInSharepointList);
        }

        [Fact]
        public async Task ProcessEventDoesNotStoreEngagementEventOnSharepoint()
        {
            var mockSharepointManager = new MockSharepointManager();
            var courseCatalog = new MockSharepointCourseCatalog();
            string courseId = "COMP0101";
            string listToStoreEvents = "Attendance Events";
            courseCatalog.mockCatalog.Add(courseId, new CourseCatalogType {ListName = listToStoreEvents, JoinWebUrl = null });
            mockSharepointManager.CreateList(listToStoreEvents, "", new List<string>());

            var eventAggregator = new EventAggregator(mockSharepointManager, courseCatalog, new InMemoryLogger<EventAggregator>());

            StudentEvent engagementEvent = new StudentEvent
            {
                CourseID = courseId,
                EventType = EventType.Engagement,
                ActivityType = "Video",
                ActivityName = "Introductory Lesson",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            };
            await eventAggregator.ProcessEvent(engagementEvent);

            var itemsInSharepointList = await mockSharepointManager.GetItemsFromList(listToStoreEvents);
            Assert.Empty(itemsInSharepointList);
        }

        [Fact]
        public async Task ProcessEventForgetToCreateList()
        {
            var mockSharepointManager = new MockSharepointManager();
            var courseCatalog = new MockSharepointCourseCatalog();
           
            string courseId = "COMP0101";
            string listToStoreEvents = "Attendance Events";
            courseCatalog.mockCatalog.Add(courseId, new CourseCatalogType {ListName = listToStoreEvents , JoinWebUrl = null });

            bool eventListExist = mockSharepointManager.CheckListExists(listToStoreEvents);
            Assert.False(eventListExist);
 
            var eventAggregator = new EventAggregator(mockSharepointManager, courseCatalog, new InMemoryLogger<EventAggregator>());

            StudentEvent eventToStore = new StudentEvent
            {
                CourseID = courseId,
                EventType = EventType.Attendance,
                ActivityType = "Video",
                ActivityName = "Introductory Lesson",
                Timestamp = DateTime.UtcNow,
                Student = new Student { Email = "a.b@example.com", FirstName = "a", LastName = "b", ID = " 0" }
            };
            await eventAggregator.ProcessEvent(eventToStore);

            bool afterProcessEventListExist = mockSharepointManager.CheckListExists(listToStoreEvents);
            Assert.True(afterProcessEventListExist);
        }
    }
}
