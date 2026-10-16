using SerieA.API.Enums;

namespace SerieA.API.Entities
{
    public class MatchCard
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Match Match { get; set; }
        public Guid TeamId { get; set; }
        public Team Team { get; set; }
        public string PlayerName { get; set; }
        public string Minute { get; set; } // We use a string for stoppage time (such as “45+2”)    
        public CardType CardType { get; set; }
    }
}
