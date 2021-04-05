using Xunit;
using Orca.Services.Adapters;
using Orca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orca.Services;
using OrcaTests.Tools;
using Orca.Entities;
using Orca.Entities.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrcaTests.Services.Adapters
{
    public class MsGraphAdapterTests
    {
        [Fact]
        public async Task ProcessTeamsMeetingWithoutJoinWebUrl()
        {
            var _graphHelper = new MockGraphHelper();
            var _logger = new InMemoryLogger<MsGraphAdapter>();
            var _courseCatalog = new MockSharepointCourseCatalog();
            var _eventAggregator = new MockEventAggregator();
            var _graphAdapter = new MsGraphAdapter(_eventAggregator, _logger, _graphHelper, _courseCatalog);

            var callId = "11111";
            string courseId = "COMP0102";
            string listToStoreEvents = "Attendance Events";
            _courseCatalog.mockCatalog.Add(courseId, new CourseCatalogType { ListName = listToStoreEvents, JoinWebUrl = "testjoinweburl.com" });

            await _graphAdapter.ProcessEvents(callId);

            Assert.Empty(_eventAggregator.processedEvents);
        }

        [Fact]
        public async Task ProcessTeamsMeeting()
        {
            var _graphHelper = new MockGraphHelper();
            var _logger = new InMemoryLogger<MsGraphAdapter>();
            var _courseCatalog = new MockSharepointCourseCatalog();
            var _eventAggregator = new MockEventAggregator();
            var _graphAdapter = new MsGraphAdapter(_eventAggregator, _logger, _graphHelper, _courseCatalog);
            
            
            string courseId = "COMP0102";
            string listToStoreEvents = "Attendance Events";
            _courseCatalog.mockCatalog.Add(courseId, new CourseCatalogType { ListName = listToStoreEvents, JoinWebUrl = "testjoinweburl.com" });
            
            // JoinWebUrl does not exist in course catalog
            var callIdJoinWebUrlNotExist = "22222";
            await _graphAdapter.ProcessEvents(callIdJoinWebUrlNotExist);
            Assert.Empty(_eventAggregator.processedEvents);

            //Only organiser is in meeting
            var callIdJoinWebUrlOrganiserOnly = "44444";
            await _graphAdapter.ProcessEvents(callIdJoinWebUrlOrganiserOnly);
            Assert.Empty(_eventAggregator.processedEvents);

            // JoinWebUrl exists in course catalog
            var callIdJoinWebUrl = "33333";
            await _graphAdapter.ProcessEvents(callIdJoinWebUrl);
            Assert.Single(_eventAggregator.processedEvents);

            //Ensure event organiser data isn't sent to event aggregator 
            Assert.True(1 == _eventAggregator.processedEvents.Count());

            //Correct student data is stored
            var processedEvent = _eventAggregator.processedEvents[0];
            Assert.Equal("COMP0102", processedEvent.CourseID);
            Assert.Equal(EventType.Attendance, processedEvent.EventType);
            Assert.Equal("Meeting", processedEvent.ActivityType);
            Assert.Equal("Weekly Lecture", processedEvent.ActivityName);
            Assert.Equal("john.doe.20@ucl.ac.uk", processedEvent.Student.Email);
            Assert.Equal("John", processedEvent.Student.FirstName);
            Assert.Equal("Doe", processedEvent.Student.LastName);
            Assert.Equal("00000", processedEvent.Student.ID);

        }
        
    }

}
