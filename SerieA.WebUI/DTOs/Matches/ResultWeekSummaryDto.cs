using SerieA.WebUI.Enums;

namespace SerieA.WebUI.DTOs.Matches
{
    public class ResultWeekSummaryDto
    {
        public int Week { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalMatches { get; set; }
        public int CompletedMatches { get; set; }
        public MatchStatus Status { get; set; }
        public bool IsValidFixture { get; set; }
        public int UniqueTeamsCount { get; set; }
    }
}
