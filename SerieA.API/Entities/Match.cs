namespace SerieA.API.Entities
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }
        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; } // Value that specifies the calendar day and time the game will be played
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string Status { get; set; } = "Oynanmadı";
        public string Stadium { get; set; }

        // A game can have more than one goal
        public ICollection<MatchGoal> MatchGoals { get; set; }
        public ICollection<MatchCard> MatchCards { get; set; }
        public ICollection<Substitution> Substitutions { get; set; }
    }
}
