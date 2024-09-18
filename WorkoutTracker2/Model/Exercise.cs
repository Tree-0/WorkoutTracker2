using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    public class Exercise
    {
        public int ExerciseId { get; set; } // primary key
        public int WorkoutId { get; set; } // foreign key
        public Workout Workout { get; set; } // navigation to parent workout


        public int NumSets => SetData.Count;
        public string Name { get; set; }
        public bool IsWeightPerLimb { get; set; }
        public bool IsBodyWeight { get; set; }
        public List<RepWeight> SetData { get; set; } // navigation to children (set info)


        // default
        public Exercise()
        {
            SetData = new List<RepWeight>();
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Name} - " +
                $"\nWeight per Limb: {IsWeightPerLimb}" +
                $"\nBW Exercise: {IsBodyWeight}" +
                $"\nW x R:" +
                $"\n");

            foreach (var repWeight in SetData)
            {
                sb.Append(repWeight.ToString() + "\n");
            }

            return sb.ToString();
        }

        public int GetTotalReps()
        {
            return SetData.Sum(rw => rw.Reps);
        }

        public float GetAverageWeight()
        {
            if (SetData.Count == 0) return 0f;

            return SetData.Average(rw => rw.Weight);

        }

        public float GetTotalWeight()
        {
            if (SetData.Count == 0) return 0f;

            return SetData.Sum(rw => rw.Weight);
        }



    }
}   
