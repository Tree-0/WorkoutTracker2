using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
using System.IO;
using System.Collections.ObjectModel;
using System.Data;

namespace WorkoutTracker2
{
    /// <summary>
    /// Interaction logic for DataEntryControl.xaml
    /// </summary>
    public partial class DataEntryControl : UserControl
    {
        private List<Exercise> ExercisesToAdd;
        private ObservableCollection<RepWeightOnly> SetDataToAdd;
        public DataEntryControl()
        {
            InitializeComponent();
            SetDataToAdd = new ObservableCollection<RepWeightOnly>();

            ExerciseDataGrid.Items.Clear();
            ExerciseDataGrid.ItemsSource = SetDataToAdd;

            ExercisesToAdd = new List<Exercise>();
        }


        /// <summary>
        /// When a date on the calendar is picked, display any workout information on that date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkoutCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var context = new WorkoutContext())
            {
                var workouts = context.Workouts
                    .Include(w => w.Exercises)
                    .ThenInclude(e => e.SetData)
                    .Where(w => w.Date == WorkoutCalendar.SelectedDate);

                if (!workouts.Any())
                {
                    SelectedWorkoutInfoBox.Text = "No workout on selected date";
                }
                else
                {
                    SelectedWorkoutInfoBox.Text = String.Empty;
                    foreach (var workout in workouts)
                    {
                        SelectedWorkoutInfoBox.Text += workout.ToString() + "\n";
                    }
                    
                }                
            }
            
        }

        
        /// <summary>
        /// When button is clicked, add a new option for exercise types to combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddExerciseTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string newExerciseName = ExerciseNameCollectionBox.Text;
            // no duplicates 
            if (!ExerciseNameCollectionBox.Items.Contains(newExerciseName))
            {
                ExerciseNameCollectionBox.Items.Add(newExerciseName);
                ExerciseNameSelectionBox.Items.Add(newExerciseName);
            }
                
        }

        
        /// <summary>
        /// When button is clicked, remove the selected option in the exercise types combo box
        /// 
        /// COULD BE RISKY
        ///     Need to decide if I want to delete all data with the associated exercise name,
        ///     or simply leave it unreferencable?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveExerciseTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string newExerciseName = ExerciseNameCollectionBox.Text;

            // Warn the user
            var messageButtonClicked = MessageBox.Show("WARNING: Removing this name will leave exercises of this type unreferencable unless you add the exact name again later", 
                "warning", MessageBoxButton.OKCancel);
            
            if (messageButtonClicked != MessageBoxResult.OK) { return; }

            ExerciseNameCollectionBox.Items.Remove(newExerciseName);
            ExerciseNameSelectionBox.Items.Remove(newExerciseName);
        }


        /// <summary>
        /// Serialize all the combo box items into a file to use later
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="filePath"></param>
        public void SaveComboBoxItems(ComboBox comboBox, string filePath)
        {
            var items = comboBox.Items.Cast<string>().ToList();  // Assuming your items are strings
            var json = JsonSerializer.Serialize(items);
            File.WriteAllText(filePath, json);
        }


        /// <summary>
        /// Deserialize all combo box items from a file on application startup
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="filePath"></param>
        public void LoadComboBoxItems(ComboBox comboBox, string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var items = JsonSerializer.Deserialize<List<string>>(json);

                comboBox.Items.Clear();
                foreach (var item in items)
                {
                    comboBox.Items.Add(item);
                }
            }
        }


        /// <summary>
        /// Add a new row for the user to edit in the set data grid when the button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewRowExerciseDataGridButton_Click(object sender, RoutedEventArgs e)
        {
            SetDataToAdd.Add(new RepWeightOnly { Reps = 0, Weight = 0 });
        }



        /// <summary>
        /// Take all data in ExerciseDataGrid, put it in an exercise object, clear table and settings for the next exercise to be input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            // get name
            string name = ExerciseNameSelectionBox.Text;

            // exercises must be named
            if (name == String.Empty)
            {
                MessageBox.Show("Select a name for the exercise");
                return;
            }

            // should not submit an empty exercise
            if (SetDataToAdd.Count == 0)
            {
                MessageBox.Show("Cannot add an empty exercise. Add sets.");
                return;
            }

            // get list of sets and clear the ones in the DataGrid
            var setData = new List<RepWeight>();
            foreach (var set in SetDataToAdd)
            {
                setData.Add(new RepWeight { Reps = set.Reps, Weight = set.Weight });
            }
            SetDataToAdd.Clear();

            // construct exercise
            var newExercise = new Exercise
            {
                IsBodyWeight = IsBodyweightCheckbox.IsChecked ?? false,
                IsWeightPerLimb = IsWeightPerLimbCheckbox.IsChecked ?? false,
                SetData = setData,
                Name = name
            };

            // add new exercise to workout list
            ExercisesToAdd.Add(newExercise);

            // add exercise name to a table of the current exercises in the workout
            ExerciseListTextBox.Text += name + "\n";      

        }


        /// <summary>
        /// Take all exercises and workout information, put it in an object, and submit it to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Workouts must have exercises
            if (ExercisesToAdd.Count == 0)
            {
                MessageBox.Show("Why would you submit an empty workout?");
                return;
            }

            // A valid date must be selected
            DateTime workoutDate;
            if (WorkoutCalendar.SelectedDate.HasValue)
            {
                workoutDate = WorkoutCalendar.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Please select a date for your workout");
                return;
            }

            string label;
            // if no name is given for the workout, make it date.ToString()
            if (WorkoutLabelTextBox.Text == String.Empty)
            {
                label = workoutDate.ToString();
            }
            else
            {
                label = WorkoutLabelTextBox.Text;
            }

            // create workout
            var workout = new Workout
            {
                Date = workoutDate,
                Exercises = ExercisesToAdd,
                Label = label,
                Description = WorkoutDescriptionTextBox.Text
            };

            // Add workout to DB
            using (var context = new WorkoutContext())
            {
                context.Workouts.Add(workout);
                int changed = context.SaveChanges();
                MessageBox.Show($"Workout Submitted. {changed} state entries written to DB");
            }

            // remove list of exercises displayed in textbox
            ExerciseListTextBox.Clear();
            // remove workout label and description
            WorkoutLabelTextBox.Clear();
            WorkoutDescriptionTextBox.Clear();
        }
    }
}
