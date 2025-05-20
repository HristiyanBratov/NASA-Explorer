using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Asteroids.Models.DTOs
{
    public class MissDistance
    {
        [JsonPropertyName("kilometers")]
        public string Kilometers { get; set; }
    }
}