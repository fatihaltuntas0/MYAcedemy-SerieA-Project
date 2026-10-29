using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.DTOs.Matches
{
    public class ResultMatchDto
    {
        public Guid Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;

        // Scores (Should be null if the game wasn't played)
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        // Home Team Information
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeTeamCode { get; set; } = string.Empty;
        public string HomeTeamLogoUrl { get; set; } = string.Empty;

        // Away Team Information
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayTeamCode { get; set; } = string.Empty;
        public string AwayTeamLogoUrl { get; set; } = string.Empty;
    }
}
