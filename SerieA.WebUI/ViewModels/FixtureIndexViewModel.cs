using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Enums;

namespace SerieA.WebUI.ViewModels
{
    public class FixtureIndexViewModel
    {
        public int SelectedWeek { get; set; }
        public List<int> AvailableWeeks { get; set; } = new();
        public List<ResultMatchDto> Matches { get; set; } = new();
        public List<ResultWeekSummaryDto> WeeksSummary { get; set; } = new();
    }
}
