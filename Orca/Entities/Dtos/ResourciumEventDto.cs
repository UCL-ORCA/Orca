using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Orca.Entities.Dtos
{
    public class ResourciumEventDto
    {
        [Required]
        [JsonPropertyName("student")]
        public ResourciumStudentDto Student { get; set; }

        /// <summary>
        /// The time at which the student registered attendance
        /// </summary>
        [Required]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

    }

    public class ResourciumStudentDto
    {
        /// <summary>
        /// Azure AD id of the student
        /// </summary>
        [Required]
        [JsonPropertyName("id")]
        public string ID { get; init; }
        
        [Required]
        [JsonPropertyName("firstname")]
        public string FirstName { get; init; }
        
        [Required]
        [JsonPropertyName("lastname")]
        public string LastName { get; init; }

        /// <summary>
        /// The fullname email of the student (i.e. john.doe.20@university.ac.uk)
        /// </summary>
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; init; }
    }

}
