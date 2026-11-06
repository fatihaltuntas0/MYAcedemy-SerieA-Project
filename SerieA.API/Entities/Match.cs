using SerieA.API.Enums;

namespace SerieA.API.Entities
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;
        public int Week { get; set; }
        public DateTime MatchDate { get; set; } // Value that specifies the calendar day and time the game will be played
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;
        public string Stadium { get; set; } = string.Empty;
        // A game can have more than one goal
        public ICollection<MatchGoal> MatchGoals { get; set; } = new List<MatchGoal>();
        public ICollection<MatchCard> MatchCards { get; set; } = new List<MatchCard>();
        public ICollection<Substitution> Substitutions { get; set; } = new List<Substitution>();
    }
}
