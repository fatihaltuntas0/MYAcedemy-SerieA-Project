namespace SerieA.WebUI.DTOs.Matches.Events
{
    public class CreateSubstitutionDto
    {
        public Guid TeamId { get; set; }
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public string Minute { get; set; } = string.Empty;
    }
}
