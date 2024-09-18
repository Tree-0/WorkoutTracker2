using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker2.Model;

namespace WorkoutTracker2.ViewModels
{
    public class BodyWeightChartViewModel
    {
        // This will hold both series (AverageWeight and TotalReps)
        public SeriesCollection CombinedSeries { get; set; }

        // List of dates for x-axis
        public List<string> DateLabels { get; set; }

        public BodyWeightChartViewModel()
        {
            CombinedSeries = new SeriesCollection();
            DateLabels = new List<string>();
        }


        /// <summary>
        /// get all bodyweight data to display in a chart
        /// </summary>
        public void LoadData()
        {
            using (var context = new WorkoutContext())
            {
                var bodyweights = context.BodyWeights.Select(bw => bw);

                var bwSeries = new LineSeries
                {
                    Title = $"Bodyweight",
                    Values = new ChartValues<float>(bodyweights.Select(bw => bw.Weight).ToList())
                };


                // Collect the dates of each entry
                DateLabels = bodyweights.Select(bw => bw.Date.ToShortDateString()).ToList();

                CombinedSeries.Clear();
                CombinedSeries.Add(bwSeries);
            }
        }
    }
}
