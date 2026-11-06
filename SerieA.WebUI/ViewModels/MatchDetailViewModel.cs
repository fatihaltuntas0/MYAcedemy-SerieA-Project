using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Enums;

namespace SerieA.WebUI.ViewModels
{
    public class MatchDetailViewModel
    {
        public Guid Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeTeamCode { get; set; } = string.Empty;
        public string HomeTeamLogoUrl { get; set; } = string.Empty;
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayTeamCode { get; set; } = string.Empty;
        public string AwayTeamLogoUrl { get; set; } = string.Empty;
        public List<MatchGoalDto> Goals { get; set; } = new();
        public List<MatchCardDto> Cards { get; set; } = new();
        public List<SubstitutionDto> Substitutions { get; set; } = new();
        // Calculated Presentation Properties
        public int HomeGoalsCount { get; set; }
        public int AwayGoalsCount { get; set; }
        public bool HasScoreMismatch { get; set; }
        public List<MatchEventItemViewModel> SortedEvents { get; set; } = new();
    }

    public class MatchEventItemViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // "goal", "card", "sub"
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
        public int SortKey { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public CardType? CardType { get; set; }
    }
}
