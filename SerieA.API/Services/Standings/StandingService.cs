using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Standings;
using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.Services.Standings
{
    public class StandingService(ApiContext _context, IMapper _mapper) : IStandingService
    {
        public async Task<ApiResponse<List<StandingRowDto>>> CalculateStandingAsync(int? upToWeek = null)
        {
            var activeTeams = await _context.Teams
                .Where(t => t.Status == true)
                .ToListAsync();

            if (!activeTeams.Any())
            {
                return ApiResponse<List<StandingRowDto>>.Success(new List<StandingRowDto>(), "Henüz aktif takım bulunmuyor.");
            }

            var standingsMap = _mapper.Map<List<StandingRowDto>>(activeTeams)
                                      .ToDictionary(x => x.TeamId);

            var query = _context.Matches
                .Where(m => m.Status == MatchStatus.Finished && m.HomeScore.HasValue && m.AwayScore.HasValue);

            if (upToWeek.HasValue)
            {
                query = query.Where(m => m.Week <= upToWeek.Value);
            }

            var completedMatches = await query
                .OrderBy(m => m.Week)
                .ThenBy(m => m.MatchDate)
                .ToListAsync();

            foreach (var match in completedMatches)
            {
                ProcessMatch(match, standingsMap);
            }

            var sortedStandings = SortAndRankStandings(standingsMap);

            return ApiResponse<List<StandingRowDto>>.Success(sortedStandings, "Puan durumu başarıyla hesaplandı.");
        }

        #region Private Helpers - Standing Calculation

        private void ProcessMatch(Match match, Dictionary<Guid, StandingRowDto> standingsMap)
        {
            if (!standingsMap.TryGetValue(match.HomeTeamId, out var home) ||
                !standingsMap.TryGetValue(match.AwayTeamId, out var away))
            {
                return;
            }

            int homeScore = match.HomeScore!.Value;
            int awayScore = match.AwayScore!.Value;

            home.Played++;
            away.Played++;

            home.GoalsFor += homeScore;
            home.GoalsAgainst += awayScore;

            away.GoalsFor += awayScore;
            away.GoalsAgainst += homeScore;

            if (homeScore > awayScore)
            {
                home.Won++;
                home.Points += 3;
                away.Lost++;

                home.Form.Add("W");
                away.Form.Add("L");
            }
            else if (homeScore < awayScore)
            {
                away.Won++;
                away.Points += 3;
                home.Lost++;

                away.Form.Add("W");
                home.Form.Add("L");
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
                home.Points++;
                away.Points++;

                home.Form.Add("D");
                away.Form.Add("D");
            }
        }

        private List<StandingRowDto> SortAndRankStandings(Dictionary<Guid, StandingRowDto> standingsMap)
        {
            foreach (var row in standingsMap.Values)
            {
                row.GoalDifference = row.GoalsFor - row.GoalsAgainst;

                if (row.Form.Count > 5)
                {
                    row.Form = row.Form.TakeLast(5).ToList();
                }
            }

            var sorted = standingsMap.Values
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].Position = i + 1;
            }

            return sorted;
        }

        #endregion
    }
}
