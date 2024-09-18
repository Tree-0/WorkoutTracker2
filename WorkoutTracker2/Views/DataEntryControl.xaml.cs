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
using WorkoutTracker2.Views;
using System.IO;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace WorkoutTracker2
{
    /// <summary>
    /// Interaction logic for DataEntryControl.xaml
    /// </summary>
    public partial class DataEntryControl : UserControl
    {
        public ObservableCollection<Exercise> ExercisesToAdd;

        private Exercise? _selectedExercise;
        public Exercise? SelectedExercise
        {
            get { return _selectedExercise; }
            set
            {
                _selectedExercise = value;
                OnPropertyChanged(nameof(SelectedExercise));  // Notify the UI of changes
            }
        }

        private ObservableCollection<RepWeightOnly> SetDataToAdd;

        public DataEntryControl()
        {
            InitializeComponent();

            DataContext = this;

            SetDataToAdd = new ObservableCollection<RepWeightOnly>();

            ExercisesToAdd = new ObservableCollection<Exercise>();

            SelectedExercise = null;

            ExerciseDataGrid.Items.Clear();
            ExerciseDataGrid.ItemsSource = SetDataToAdd;
            ExerciseListView.ItemsSource = ExercisesToAdd;
            ExerciseListView.SelectedItem = SelectedExercise;

        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// When a date on the calendar is picked, display any workout and bodyweight information on that date
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

                var bodyweight = context.BodyWeights
                    .Where(bw => bw.Date == WorkoutCalendar.SelectedDate)
                    .Select(bw => bw.Weight);

                if (!workouts.Any() && !bodyweight.Any())
                {
                    SelectedWorkoutInfoBox.Text = "No info on selected date";
                }
                else
                {
                    SelectedWorkoutInfoBox.Text = String.Empty;

                    // display any bodyweight information
                    if (bodyweight.Any())
                    {
                        SelectedWorkoutInfoBox.Text += "BW: " + bodyweight.First().ToString() + "\n";
                    }

                    if (workouts.Any())
                    {
                        foreach (var workout in workouts)
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

            // get list of sets
            var setData = GetSetData();

            // construct exercise
            var newExercise = new Exercise
            {
                IsBodyWeight = IsBodyweightCheckbox.IsChecked ?? false,
                IsWeightPerLimb = IsWeightPerLimbCheckbox.IsChecked ?? false,
                SetData = setData,
                Name = name
            };

            // clear temp data, set selections to null or false
            SetDataToAdd.Clear();
            ExerciseNameSelectionBox.SelectedItem = null;
            ExerciseListView.SelectedItem = null;

            // add new exercise to workout list
            ExercisesToAdd.Add(newExercise);

            IsBodyweightCheckbox.IsChecked = false;
            IsWeightPerLimbCheckbox.IsChecked = false;

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
                Exercises = ExercisesToAdd.ToList(),
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

            ClearWorkoutButton_Click(sender, e);
        }


        /// <summary>
        /// Reset all data input fields without submitting the workout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            //ExerciseListTextBox.Clear();
            WorkoutLabelTextBox.Clear();
            WorkoutDescriptionTextBox.Clear();
            SetDataToAdd.Clear();
            ExercisesToAdd.Clear();
            IsBodyweightCheckbox.IsChecked = false;
            IsWeightPerLimbCheckbox.IsChecked = false;
            ExerciseNameSelectionBox.SelectedItem = null;
            ExerciseListView.SelectedItem = null;
        }


        /// <summary>
        /// Delete the workout on a selected date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Delete Workouts on {WorkoutCalendar.SelectedDate.ToString()}?", "Confirm",
                MessageBoxButton.OKCancel);

            // User can decide not to delete
            if (messageBoxResult == MessageBoxResult.Cancel)
                return;

            // Delete all workouts on the selected date
            using (var context = new WorkoutContext())
            {
                var workouts = context.Workouts.Where(w => w.Date == WorkoutCalendar.SelectedDate);
                workouts.ExecuteDelete();
                context.SaveChanges();
            }

        }


        /// <summary>
        /// When a new exercise is selected, call loading method to display that exercise's information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExerciseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedExercise();
        }


        /// <summary>
        /// Take the SelectedExercise and populate all the fields, which you can then edit
        /// </summary>
        private void LoadSelectedExercise()
        {
            if (SelectedExercise is null)
                return;

            // clear anything previous
            SetDataToAdd.Clear();

            // repopulate datagrid
            foreach (var rw in SelectedExercise.SetData)
            {
                SetDataToAdd.Add(new RepWeightOnly() { Reps = rw.Reps, Weight = rw.Weight });
            }

            // set combobox selection to the name of the exercise
            ExerciseNameSelectionBox.SelectedItem = SelectedExercise.Name;
            


            IsBodyweightCheckbox.IsChecked = SelectedExercise.IsBodyWeight;
            IsWeightPerLimbCheckbox.IsChecked = SelectedExercise.IsWeightPerLimb;
        }

        
        /// <summary>
        /// When the value of a cell is changed, update the set data for that selected exercise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExerciseDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (SelectedExercise is null)
                return;

            SelectedExercise.SetData = GetSetData();
        }


        /// <summary>
        /// when the exercise name is changed (new exercise being created) save the previous exercise's set data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExerciseNameSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedExercise is null)
                return;

            SelectedExercise.SetData = GetSetData();
        }


        /// <summary>
        /// Return a list of the sets (reps, weights) currently in the datagrid 
        /// </summary>
        /// <returns></returns>
        private List<RepWeight> GetSetData()
        {
            // save the edited cells to the exercise 
            var setData = new List<RepWeight>();
            foreach (var set in SetDataToAdd)
            {
                setData.Add(new RepWeight { Reps = set.Reps, Weight = set.Weight });
            }

            return setData;
        }


        /// <summary>
        /// Add the BodyWeight in the textbox to the DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBodyWeightButton_Click(object sender, RoutedEventArgs e)
        {
            // check for valid number
            if(float.TryParse(BodyWeightTextBox.Text, out var bw))
            {
                using (var context = new WorkoutContext())
                {
                    // add a new bodyweight to the database on the specified date
                    context.BodyWeights.Add(new BodyWeight 
                    { 
                        Weight = bw,
                        Date = WorkoutCalendar.SelectedDate ?? DateTime.Now.Date, 
                    });

                    int changed = context.SaveChanges();
                    MessageBox.Show($"Bodyweight Submitted. {changed} state entries written to DB");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid number", "Invalid Input", MessageBoxButton.OK);
            }

        }


        /// <summary>
        /// Delete the Bodyweight on the selected date.
        /// 
        /// NOTE: Right now, multiple bodyweights can be set on the same date, but only the first one will ever be displayed (because why would you need multiple?)
        /// Eventually I should make it so that you can only have one, via replacement, but for now to display a new bodyweight, the delete button will 
        /// remove ALL entries on the specified date, and then you can add a new one. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBodyWeightButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Delete BodyWeight on {WorkoutCalendar.SelectedDate.ToString()}?", "Confirm", MessageBoxButton.OKCancel);

            // User can decide not to delete
            if (messageBoxResult == MessageBoxResult.Cancel)
                return;

            // remove bodyweight
            using (var context = new WorkoutContext())
            {
                var bw = context.BodyWeights.Where(bw => bw.Date == WorkoutCalendar.SelectedDate).Select(bw => bw);

                bw.ExecuteDelete();

                context.SaveChanges();
            }
        }
    }
}
