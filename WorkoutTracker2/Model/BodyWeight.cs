using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    public class BodyWeight
    {
        public int BodyWeightId { get; set; } // Primary Key
        public DateTime Date { get; set; } // Date when the bodyweight was recorded
        public float Weight { get; set; } // The bodyweight value
    }
}
