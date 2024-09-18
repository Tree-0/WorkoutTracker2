using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    /// <summary>
    /// This is the class used for Entity Framework, and represents a table in the database
    /// 
    /// Class representing one set of an exercise, with a number of reps, and the weight. 
    /// </summary>
    public class RepWeight
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
