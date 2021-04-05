using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Orca.Entities.Dtos
{
    public class CaliperEventBatchDto
    {
        [Required]
        [JsonPropertyName("data")]
        public List<CaliperEventDto> Data { get; set; }
    }
}
