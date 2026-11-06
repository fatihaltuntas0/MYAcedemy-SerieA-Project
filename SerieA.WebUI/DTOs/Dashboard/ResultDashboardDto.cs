using SerieA.WebUI.DTOs.Standings;

namespace SerieA.WebUI.DTOs.Dashboard
{
    public class ResultDashboardDto
    {
        // KPI Summary
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
