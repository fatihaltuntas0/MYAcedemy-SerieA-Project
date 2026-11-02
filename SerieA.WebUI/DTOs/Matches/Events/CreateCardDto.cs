using SerieA.WebUI.Enums;

namespace SerieA.WebUI.DTOs.Matches.Events
{
    public class CreateCardDto
    {
        public Guid TeamId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
        public CardType CardType { get; set; }
    }
}
