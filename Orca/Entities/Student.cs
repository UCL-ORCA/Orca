using System;
using System.ComponentModel.DataAnnotations;

namespace Orca.Entities
{
    public class Student
    {
        [Required]
        public string ID { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }


        public override string ToString()
        {
            return $"{ID}-{FirstName}-{LastName}-{Email}";
        }
    }
}
