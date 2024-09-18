using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker2.Model;

namespace WorkoutTracker2.ViewModels
{
    public class WeightChartViewModel
    {
        // This will hold both series (AverageWeight and TotalReps)
        public SeriesCollection CombinedSeries { get; set; }

        // List of dates for x-axis
        public List<string> DateLabels { get; set; }

        public WeightChartViewModel()
        {
            CombinedSeries = new SeriesCollection();
            DateLabels = new List<string>();
        }

        public void LoadData(string exerciseName)
        {
            using (var context = new WorkoutContext())
            {
                // Fetch workouts with the desired exercise, including related exercises and rep weights
                var exerciseData = context.Workouts
                    .Where(w => w.Exercises.Any(e => e.Name == exerciseName))
                    .Select(w => new
                    {
                        w.Date,  // Workout date
                        AverageWeight = w.Exercises
                            .Where(e => e.Name == exerciseName)
                            .SelectMany(e => e.SetData)
                            .Average(rw => rw.Weight),  // Average weight
                        TotalReps = w.Exercises
                            .Where(e => e.Name == exerciseName)
                            .SelectMany(e => e.SetData)
                            .Sum(rw => rw.Reps),  // Total reps
                        Sets = w.Exercises
                            .Where(e => e.Name == exerciseName)
                            .Select(e => e.SetData.Count())
                            .FirstOrDefault() // Sets in exercise
                    })
                    .ToList();

                // Collect the dates of each entry
                DateLabels = exerciseData.Select(ed => ed.Date.ToShortDateString()).ToList();


                // LineSeries for Average Weight
                var averageWeightSeries = new LineSeries
                {
                    Title = $"Average Weight for {exerciseName}",
                    Values = new ChartValues<float>(exerciseData.Select(aw => (float)aw.AverageWeight)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10
                };

                // LineSeries for Total Reps
                var totalRepsSeries = new LineSeries
                {
                    Title = $"Total Reps for {exerciseName}",
                    Values = new ChartValues<int>(exerciseData.Select(er => er.TotalReps)),
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 10
                };

                var numSetsSeries = new LineSeries
                { 
                    Title = $"Number of Sets for {exerciseName}",
                    Values = new ChartValues<int>(exerciseData.Select(es => es.Sets)),
                };


                // Clear the combined series and add both series
                CombinedSeries.Clear();
                CombinedSeries.Add(averageWeightSeries);
                CombinedSeries.Add(totalRepsSeries);
                CombinedSeries.Add(numSetsSeries);
            }
        }
    }
}