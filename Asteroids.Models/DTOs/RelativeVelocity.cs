using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Asteroids.Models.DTOs
{
    public class RelativeVelocity
    {
        [JsonPropertyName("kilometers_per_hour")]
        public string KilometersPerHour { get; set; }
    }
}