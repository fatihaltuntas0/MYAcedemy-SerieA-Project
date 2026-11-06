namespace SerieA.API.DTOs.Teams
{
    public class ResultTeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;
        public int StadiumCapacity { get; set; }
        public DateTime FoundedYear { get; set; }
        public string ColorOne { get; set; } = string.Empty;
        public string ColorTwo { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
