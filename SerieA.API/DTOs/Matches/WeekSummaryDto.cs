using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.DTOs.Matches
{
    public class WeekSummaryDto
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
