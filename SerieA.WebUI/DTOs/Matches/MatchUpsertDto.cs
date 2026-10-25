namespace SerieA.WebUI.DTOs.Matches
{
    public class MatchUpsertDto
    {
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string Status { get; set; } = "Oynanmadı";
        public string Stadium { get; set; } = string.Empty;
    }
}
