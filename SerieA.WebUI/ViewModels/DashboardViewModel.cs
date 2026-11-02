using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.DTOs.Standings;

namespace SerieA.WebUI.ViewModels
{
    public class DashboardViewModel
    {
        // KPI Summary Informations
        public int ActiveTeamCount { get; set; }
        public int FinishedMatchCount { get; set; }
        public int TotalMatchCount { get; set; }
        public int TotalGoals { get; set; }
        public double AverageGoalsPerMatch { get; set; }
        public int NextWeek { get; set; }

        // Sections
        public List<ResultMatchDto> RecentMatches { get; set; } = new();
        public List<ResultMatchDto> UpcomingMatches { get; set; } = new();
        public List<ResultStandingRowDto> TopStandings { get; set; } = new();
    }
}
