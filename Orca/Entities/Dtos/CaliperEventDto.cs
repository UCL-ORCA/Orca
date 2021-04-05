using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Orca.Entities.Dtos
{
    public class CaliperEventDto
    {

        [Required]
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        [Required]
        [JsonPropertyName("actor")]
        public CaliperActor Actor { get; set; }

        [Required]
        [JsonPropertyName("object")]
        public CaliperObject Object { get; set; }

        [Required]
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [Required]
        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; }

        [Required]
        [JsonPropertyName("group")]
        public CaliperGroup Group { get; set; }

        [Required]
        [JsonPropertyName("membership")]
        public CaliperActorMembership Membership { get; set; }
    }

    public class CaliperActor
    {
        [Required]
        [JsonPropertyName("@id")]
        public string Id { get; set; }


        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("@type")]
        public string ActorType { get; set; }

        [Required]
        [JsonPropertyName("extensions")]
        public CaliperActorExtensions Extensions { get; set; }
    }

    public class CaliperActorExtensions
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class CaliperObject
    {

        [Required]
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        [Required]
        [JsonPropertyName("@type")]
        public string ObjectType { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class CaliperGroup
    {
        [Required]
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        [Required]
        [JsonPropertyName("@type")]
        public string GroupType { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class CaliperActorMembership
    {
        [Required]
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
    }
}
