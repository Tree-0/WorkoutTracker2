using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    internal class RepWeight
    {
        public int RepWeightId { get; set; } // primary key
        public int ExerciseId { get; set; } // foreign key
        public Exercise Exercise { get; set; } // navigation

        public int Reps { get; set; } // repetitions
        public float Weight { get; set; } // weight used


        public override string ToString()
        {
            return $"{Weight} x {Reps}";
        }
    }
}
