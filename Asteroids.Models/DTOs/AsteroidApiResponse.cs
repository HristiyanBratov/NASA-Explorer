using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Asteroids.Models.DTOs
{
    public class AsteroidApiResponse
    {
        [JsonPropertyName("near_earth_objects")]
        public Dictionary<string, List<NeoObject>> NearEarthObjects { get; set; }
    }
}