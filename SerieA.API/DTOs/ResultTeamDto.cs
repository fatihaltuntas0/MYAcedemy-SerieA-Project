namespace SerieA.API.DTOs
{
    public class ResultTeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Stadium { get; set; }
        public int StadiumCapacity { get; set; }
        public DateTime FoundedYear { get; set; }
        public string ColorOne { get; set; }
        public string ColorTwo { get; set; }
        public string Status { get; set; }
    }
}
