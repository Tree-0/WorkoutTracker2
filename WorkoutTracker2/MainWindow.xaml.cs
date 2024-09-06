using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkoutTracker2.Model;

namespace WorkoutTracker2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Workout workout = new Workout();
            workout.WorkoutId = 1;
            workout.Label = "test";
            workout.Date = DateTime.Now;
            workout.Description = "test description";

            Exercise exercise_one = new Exercise();
            exercise_one.ExerciseId = 1;
            exercise_one.WorkoutId = 1;
            exercise_one.Workout = workout;
            
            RepWeight repWeight_one = new RepWeight();
            repWeight_one.RepWeightId = 1;
            repWeight_one.Reps = 10;
            repWeight_one.Weight = 100f;
            repWeight_one.ExerciseId = 1;
            repWeight_one.Exercise = exercise_one;

            RepWeight repWeight_two = new RepWeight();
            repWeight_two.RepWeightId = 2;
            repWeight_two.Reps = 20;
            repWeight_two.Weight = 200f;
            repWeight_two.ExerciseId = 1;
            repWeight_two.Exercise = exercise_one;

            workout.Exercises = new List<Exercise> { exercise_one };
            exercise_one.SetData = new List<RepWeight> { repWeight_one, repWeight_two };

            using (var workoutContext = new WorkoutContext())
            {
                workoutContext.Add(workout);
                workoutContext.SaveChanges();
            }

        }
    }
}