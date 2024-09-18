using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using WorkoutTracker2.ViewModels;

namespace WorkoutTracker2
{
    /// <summary>
    /// Interaction logic for GraphingControl.xaml
    /// </summary>
    public partial class GraphingControl : UserControl
    {
        public WeightChartViewModel ExerciseChartViewModel {get; set;}
        public BodyWeightChartViewModel BodyWeightChartViewModel {get; set;}
        public GraphingControl()
        {
            InitializeComponent();

            ExerciseChartViewModel = new WeightChartViewModel();
            BodyWeightChartViewModel = new BodyWeightChartViewModel();

            ExerciseChartView.DataContext = ExerciseChartViewModel;
            BodyWeightChartView.DataContext = BodyWeightChartViewModel;
            

            
        }

        /// <summary>
        /// When graph button is clicked, load the chosen exercise name, query for exercises with that name, and load the weight lifted into the graph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExerciseToGraphNameSelectionBox.SelectedItem == null)
            {
                return;
            }

            string exerciseName = ExerciseToGraphNameSelectionBox.Text;
            ExerciseChartViewModel.LoadData(exerciseName);
            BodyWeightChartViewModel.LoadData();
        }
    }
}
