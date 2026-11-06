using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Enums;

namespace SerieA.WebUI.ViewModels
{
    public class WeekIndexViewModel
    {
        public List<ResultWeekSummaryDto> Weeks { get; set; } = new();

        // KPI properties provided directly by the ViewModel rather than being calculated in the View:
        public int TotalWeeks => Weeks.Count;
        public int CompletedWeeks => Weeks.Count(w => w.Status == MatchStatus.Finished);
        public int LiveOrPlannedWeeks => TotalWeeks - CompletedWeeks;
        public int ValidWeeksCount => Weeks.Count(w => w.IsValidFixture);
    }
}
