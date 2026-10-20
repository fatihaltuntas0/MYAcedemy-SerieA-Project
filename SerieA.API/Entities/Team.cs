namespace SerieA.API.Entities
{
    public class Team
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
        public bool Status { get; set; }

        // A team can have more than one game and more than one goal
        public ICollection<Match> HomeMatches { get; set; }
        public ICollection<Match> AwayMatches { get; set; }
        public ICollection<MatchGoal> MatchGoals { get; set; }
    }
}
