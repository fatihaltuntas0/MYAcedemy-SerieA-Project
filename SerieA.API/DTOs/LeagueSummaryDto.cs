namespace SerieA.API.DTOs
{
    public class LeagueSummaryDto
    {
        public int ActiveTeamCount { get; set; }
        public int MaxTeamLimit { get; set; } = 20;
        public int CityCount { get; set; }
        public int TotalStadiumCapacity { get; set; }
        public string OldestClubName { get; set; }
        public int OldestClubFoundedYear { get; set; }
    }
}
