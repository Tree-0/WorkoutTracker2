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

namespace WorkoutTracker2
{
    /// <summary>
    /// Interaction logic for DataEntryControl.xaml
    /// </summary>
    public partial class DataEntryControl : UserControl
    {
        public DataEntryControl()
        {
            InitializeComponent();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

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
                        SelectedWorkoutInfoBox.Text += workout.ToString();
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
            string newExerciseName = ExerciseNameSelectionBox.Text;
            // no duplicates 
            if (!ExerciseNameSelectionBox.Items.Contains(newExerciseName))
                ExerciseNameSelectionBox.Items.Add(newExerciseName);
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
            string newExerciseName = ExerciseNameSelectionBox.Text;

            // Warn the user
            var messageButtonClicked = MessageBox.Show("WARNING: Removing this name will leave exercises of this type unreferencable unless you add the exact name again later", 
                "warning", MessageBoxButton.OKCancel);
            
            if (messageButtonClicked != MessageBoxResult.OK) { return; }

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


    }
}
