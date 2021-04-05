
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Entities
{
    public class StudentEvent
    {
        [Required]
        public string CourseID { get; init; }
        [Required]
        public DateTime Timestamp { get; init; }
        [Required]
        public Student Student { get; init; }
        [Required]
        public EventType EventType { get; init; }
        public string ActivityName { get; init; }
        public string ActivityType { get; init; }

        public override string ToString()
        {
            return $"{{CourseID: {CourseID}, Student: {{ {Student} }}, EventType: {EventType}, ActivityType: {ActivityType}, ActivityName: {ActivityName}, Timestamp: {Timestamp} }}";
        }

    }

    public enum EventType
    {
        Engagement,
        Attendance
    }
}
