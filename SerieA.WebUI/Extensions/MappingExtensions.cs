using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Enums;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Extensions
{
    public static class MappingExtensions
    {
        public static MatchDetailViewModel ToViewModel(this ResultMatchDetailDto dto)
        {
            var homeGoals = dto.Goals.Count(g => g.TeamId == dto.HomeTeamId);
            var awayGoals = dto.Goals.Count(g => g.TeamId == dto.AwayTeamId);
            var mismatch = dto.Status == MatchStatus.Finished && ((dto.HomeScore ?? 0) != homeGoals || (dto.AwayScore ?? 0) != awayGoals);

            var events = new List<MatchEventItemViewModel>();

            foreach (var g in dto.Goals)
            {
                events.Add(new MatchEventItemViewModel
                {
                    Id = g.Id,
                    Type = "goal",
                    TeamId = g.TeamId,
                    TeamName = g.TeamName,
                    Minute = g.Minute,
                    SortKey = ParseMinute(g.Minute),
                    PlayerName = g.PlayerName
                });
            }

            foreach (var c in dto.Cards)
            {
                events.Add(new MatchEventItemViewModel
                {
                    Id = c.Id,
                    Type = "card",
                    TeamId = c.TeamId,
                    TeamName = c.TeamName,
                    Minute = c.Minute,
                    SortKey = ParseMinute(c.Minute),
                    PlayerName = c.PlayerName,
                    CardType = c.CardType
                });
            }

            foreach (var s in dto.Substitutions)
            {
                events.Add(new MatchEventItemViewModel
                {
                    Id = s.Id,
                    Type = "sub",
                    TeamId = s.TeamId,
                    TeamName = s.TeamName,
                    Minute = s.Minute,
                    SortKey = ParseMinute(s.Minute),
                    PlayerName = s.PlayerIn,
                    PlayerOut = s.PlayerOut
                });
            }

            return new MatchDetailViewModel
            {
                Id = dto.Id,
                Week = dto.Week,
                MatchDate = dto.MatchDate,
                Stadium = dto.Stadium,
                Status = dto.Status,
                HomeScore = dto.HomeScore,
                AwayScore = dto.AwayScore,
                HomeTeamId = dto.HomeTeamId,
                HomeTeamName = dto.HomeTeamName,
                HomeTeamCode = dto.HomeTeamCode,
                HomeTeamLogoUrl = dto.HomeTeamLogoUrl,
                AwayTeamId = dto.AwayTeamId,
                AwayTeamName = dto.AwayTeamName,
                AwayTeamCode = dto.AwayTeamCode,
                AwayTeamLogoUrl = dto.AwayTeamLogoUrl,
                Goals = dto.Goals,
                Cards = dto.Cards,
                Substitutions = dto.Substitutions,
                HomeGoalsCount = homeGoals,
                AwayGoalsCount = awayGoals,
                HasScoreMismatch = mismatch,
                SortedEvents = events.OrderBy(e => e.SortKey).ToList()
            };
        }

        private static int ParseMinute(string min)
        {
            if (string.IsNullOrWhiteSpace(min)) return 0;
            var parts = min.Split('+');
            if (int.TryParse(parts[0], out var baseMin))
            {
                var added = parts.Length > 1 && int.TryParse(parts[1], out var ext) ? ext : 0;
                return baseMin * 100 + added;
            }
            return 0;
        }
    }
}
