using SerieA.WebUI.Enums;

namespace SerieA.WebUI.DTOs.Matches
{
    public class MatchGoalDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
    }

    public class MatchCardDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
        public CardType CardType { get; set; }
    }

    public class SubstitutionDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
    }

    public class ResultMatchDetailDto : ResultMatchDto
    {
        public List<MatchGoalDto> Goals { get; set; } = new();
        public List<MatchCardDto> Cards { get; set; } = new();
        public List<SubstitutionDto> Substitutions { get; set; } = new();
    }
}
