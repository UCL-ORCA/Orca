using Orca.Services;
using Microsoft.Extensions.Logging;
using Orca.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Orca.Database;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Orca.Controllers
{
    // -- /api/events/mock
    [ApiController]
    [Route("api/events/mock")]
    public class MockEventController : ControllerBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly bool _enabled;

        public MockEventController(IEventAggregator eventAggregator, IWebHostEnvironment env)
        {
            // Passing event aggregator to event controller.
            _eventAggregator = eventAggregator;
            _enabled = env.IsDevelopment();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateEvent()
        {
            if (!_enabled)
            {
                return NotFound();
            }
            StudentEvent testEvent = new StudentEvent
            {
                CourseID = "COMP0199", // Course ID Upper case.
                Timestamp = DateTime.UtcNow,
                EventType = EventType.Attendance,
                ActivityType = "Video",
                ActivityName = "Weekly Lecture",
                Student = new Student 
                { 
                    Email = "vcdin.zard@example.com",
                    FirstName = "Vcdin",
                    LastName = "Zard",
                    ID = "202001955"
                }
            };
            await _eventAggregator.ProcessEvent(testEvent);
            return Ok(testEvent);
        }

    }
}
