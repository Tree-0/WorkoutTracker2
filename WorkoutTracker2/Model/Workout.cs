using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    public class Workout
    {
        public int WorkoutId { get; set; } // primary key
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public List<Exercise> Exercises { get; set; } // navigation to exercise children
        public int numExercises => Exercises.Count;


        // Default
        public Workout()
        {
            Exercises = new List<Exercise>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"{Label}\n");
            sb.Append($"Description:\n{Description}\n");

            foreach( var exercise in Exercises )
                { sb.Append( exercise.ToString() ); }

            return sb.ToString();
        }

    }
}
