namespace SerieA.API.Entities
{
    public class MatchGoal
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public Guid TeamId { get; set; }
        public Team Team { get; set; } = null!;
        public string PlayerName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;  // We use a string for stoppage time (such as “45+2”)

    }
}
