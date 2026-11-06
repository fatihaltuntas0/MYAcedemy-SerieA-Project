using SerieA.WebUI.DTOs.Dashboard;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.DTOs.Standings;

namespace SerieA.WebUI.ViewModels
{
    public class DashboardViewModel
    {
        public int ActiveTeamCount { get; set; }
        public int FinishedMatchCount { get; set; }
        public int TotalMatchCount { get; set; }
        public int TotalGoals { get; set; }
        public double AverageGoalsPerMatch { get; set; }
        public int NextWeek { get; set; }
        public List<ResultMatchDto> RecentMatches { get; set; } = new();
        public List<ResultMatchDto> UpcomingMatches { get; set; } = new();
        public List<ResultStandingRowDto> TopStandings { get; set; } = new();

        public DashboardViewModel() { }
        public DashboardViewModel(ResultDashboardDto dto)
        {
            ActiveTeamCount = dto.ActiveTeamCount;
            FinishedMatchCount = dto.FinishedMatchCount;
            TotalMatchCount = dto.TotalMatchCount;
            TotalGoals = dto.TotalGoals;
            AverageGoalsPerMatch = dto.AverageGoalsPerMatch;
            NextWeek = dto.NextWeek;
            RecentMatches = dto.RecentMatches ?? new();
            UpcomingMatches = dto.UpcomingMatches ?? new();
            TopStandings = dto.TopStandings ?? new();
        }
    }
}
