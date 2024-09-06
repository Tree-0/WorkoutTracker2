using Microsoft.EntityFrameworkCore;
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

            // Setup anything that needs to be setup or saved when application is started and stopped
            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

            Workout workout = new Workout
            {
                Label = "test",
                Date = DateTime.Now.Date,
                Description = "test description"
            };

            Exercise exercise_one = new Exercise
            {
                Workout = workout,
                Name = "Exercise 1",
                IsBodyWeight = false
            };

            RepWeight repWeight_one = new RepWeight
            {
                Reps = 10,
                Weight = 100f,
                Exercise = exercise_one
            };

            RepWeight repWeight_two = new RepWeight
            {
                Reps = 20,
                Weight = 200f,
                Exercise = exercise_one
            };

            workout.Exercises = new List<Exercise> { exercise_one };
            exercise_one.SetData = new List<RepWeight> { repWeight_one, repWeight_two };

            try
            {
                using (var workoutContext = new WorkoutContext())
                {
                    
                    workoutContext.Workouts.Where(w => w.Date == workout.Date).ExecuteDelete();
                    workoutContext.Add(workout);
                    workoutContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }

        }


        /// <summary>
        /// Perform setup on window load and any saving/teardown needed on window close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, EventArgs e)
        {
            // save the items in the exercise selection comboBox to a JSON file
            DataEntryControl.SaveComboBoxItems(DataEntryControl.ExerciseNameSelectionBox, "exerciseComboBox_items");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load the items from a JSON file to the exercise selection comboBox
            DataEntryControl.LoadComboBoxItems(DataEntryControl.ExerciseNameSelectionBox, "exerciseComboBox_items");
        }
    }
}