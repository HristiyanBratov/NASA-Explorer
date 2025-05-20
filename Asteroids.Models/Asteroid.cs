using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Models
{
    public class Asteroid
    {
        public string Name { get; set; }

        public string CloseApproachDate { get; set; }

        public double EstimatedDiameterKm { get; set; }

        public double RelativeVelocityKph { get; set; }

        public double MissDistanceKm { get; set; }

        public bool IsPotentiallyHazardous { get; set; }
    }
}