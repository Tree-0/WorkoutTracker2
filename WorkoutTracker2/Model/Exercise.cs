using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    internal class Exercise
    {
        public int ExerciseId { get; set; } // primary key
        public int WorkoutId { get; set; } // foreign key
        public Workout Workout { get; set; } // navigation to parent workout


        public int NumSets => SetData.Count;
        public string Name { get; set; }
        public bool IsWeightPerLimb { get; set; }
        public List<RepWeight> SetData { get; set; } // navigation to children (set info)


        // default
        public Exercise()
        {
            SetData = new List<RepWeight>();
        }



    }
}   
