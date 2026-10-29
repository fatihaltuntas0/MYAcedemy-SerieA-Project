using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.DTOs.Matches
{
    public class MatchUpsertDto
    {
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; } // Value that specifies the calendar day and time the game will be played
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;
        public string Stadium { get; set; }
    }
}
