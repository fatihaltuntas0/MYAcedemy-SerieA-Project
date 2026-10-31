namespace SerieA.WebUI.DTOs.Standings
{
    public class ResultStandingRowDto
    {
        public int Position { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamCode { get; set; } = string.Empty;
        public string TeamLogoUrl { get; set; } = string.Empty;
        public string TeamColorOne { get; set; } = string.Empty;
        public string TeamColorTwo { get; set; } = string.Empty;

        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }

        public List<string> Form { get; set; } = new();
    }
}
