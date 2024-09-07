using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    /// <summary>
    /// THIS CLASS IS NOT PART OF ENTITY FRAMEWORK
    /// 
    /// Used only for the data binding for the ExerciseDataGrid in DataEntryControl.XAML
    /// because tuples were not working
    /// </summary>
    class RepWeightOnly
    {
        public int Reps { get; set; }
        public float Weight { get; set; }
    }
}
