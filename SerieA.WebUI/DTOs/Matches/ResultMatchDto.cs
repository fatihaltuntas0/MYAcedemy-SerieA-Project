namespace SerieA.WebUI.DTOs.Matches
{
    public class ResultMatchDto
    {
        public Guid Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string Status { get; set; } = "Oynanmadı";
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeTeamCode { get; set; } = string.Empty;
        public string HomeTeamLogoUrl { get; set; } = string.Empty;
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayTeamCode { get; set; } = string.Empty;
        public string AwayTeamLogoUrl { get; set; } = string.Empty;
    }
}
