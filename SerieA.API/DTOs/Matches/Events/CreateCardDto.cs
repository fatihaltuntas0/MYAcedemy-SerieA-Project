using SerieA.API.Enums;
using System.Reflection.Metadata.Ecma335;

namespace SerieA.API.DTOs.Matches.Events
{
    public class CreateCardDto
    {
        public Guid TeamId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
        public CardType CardType { get; set; }
    }
}
