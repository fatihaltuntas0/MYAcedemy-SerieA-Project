namespace SerieA.API.Entities
{
    public class Substitution
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; }
        public Guid TeamId { get; set; }
        public Team Team { get; set; }
        public string PlayerIn { get; set; }
        public string PlayerOut { get; set; }
        public string Minute { get; set; } // We use a string for stoppage time (such as “45+2”)
    }
}
