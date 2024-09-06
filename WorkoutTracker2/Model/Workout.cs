using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    internal class Workout
    {
        public int WorkoutId { get; set; } // primary key
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public List<Exercise> Exercises { get; set; } // navigation to exercise children


        // Default
        public Workout()
        {
            Exercises = new List<Exercise>();
        }


        public Workout(DateTime date, string label, string description, List<Exercise> exercises, int workoutId)
        {
            this.Date = date;
            this.Label = label;
            this.Description = description;
            this.Exercises = exercises;
            this.WorkoutId = workoutId;
        }
        
    }
}
