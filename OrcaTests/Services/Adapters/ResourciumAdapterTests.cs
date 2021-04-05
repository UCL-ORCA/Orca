using Xunit;
using Orca.Services.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Entities.Dtos;

namespace OrcaTests.Services.Adapters
{
    public class ResourciumAdapterTests
    {
        [Fact]
        public async Task ConvertsResourciumEventToStudentEvent()
        {

            MockEventAggregator mockAggregator = new MockEventAggregator();
            var resourciumAdapter = new ResourciumAdapter(mockAggregator);
            var incomingEvent = new ResourciumEventDto
            {
                Timestamp = DateTime.Today,
                Student = new ResourciumStudentDto { ID = "id", Email = "a.b@example.com", FirstName = "A", LastName = "B" }
            };

            await resourciumAdapter.ProcessEvent(incomingEvent);

            var processedEvents = mockAggregator.processedEvents;
            Assert.Single(processedEvents);
            var convertedEvent = processedEvents[0];
            Assert.Equal(incomingEvent.Timestamp, convertedEvent.Timestamp);
            Assert.Equal(incomingEvent.Student.ID, convertedEvent.Student.ID);
            Assert.Equal(incomingEvent.Student.Email, convertedEvent.Student.Email);
            Assert.Equal(incomingEvent.Student.FirstName, convertedEvent.Student.FirstName);
            Assert.Equal(incomingEvent.Student.LastName, convertedEvent.Student.LastName);
            Assert.Equal(Orca.Entities.EventType.Attendance, convertedEvent.EventType);
            Assert.Equal(ResourciumAdapter.MANUAL_ATTENDANCE_COURSE_ID, convertedEvent.CourseID);
        }
    }
}
