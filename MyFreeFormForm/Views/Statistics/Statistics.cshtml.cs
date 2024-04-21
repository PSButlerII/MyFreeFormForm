using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFreeFormForm.Models;
using System.Collections.Generic; // Include this for List<>

namespace MyFreeFormForm.Views.Statistics
{
    public class StatisticsModel : PageModel
    {
        public FormStatisticsViewModel ViewModel { get; set; }

        public void OnGet()
        {
            // Simulating data retrieval and transformation
            // In reality, you'd fetch this data from your database or service layer
            var statisticsData = new List<FormStatisticsViewModel.FormStatistic>
            {
                new FormStatisticsViewModel.FormStatistic { FormName = "Form 1", Count = 10 },
                new FormStatisticsViewModel.FormStatistic { FormName = "Form 2", Count = 20 }
                // Populate with actual data
            };

            ViewModel = new FormStatisticsViewModel
            {
                Statistics = statisticsData
            };
        }
    }
}
