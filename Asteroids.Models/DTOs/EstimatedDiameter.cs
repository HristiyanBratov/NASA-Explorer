using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Asteroids.Models.DTOs
{
    public class EstimatedDiameter
    {
        [JsonPropertyName("kilometers")]
        public DiameterRange Kilometers { get; set; }
    }
}