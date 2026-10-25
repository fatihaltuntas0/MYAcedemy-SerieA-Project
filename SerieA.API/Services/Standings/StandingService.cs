using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Standings;

namespace SerieA.API.Services.Standings
{
    public class StandingService(ApiContext _context, IMapper _mapper) : IStandingService
    {
        public async Task<ApiResponse<List<StandingRowDto>>> CalculateStandingAsync()
        {
            // Get active teams
            var activeTeams = await _context.Teams
                .Where(t => t.Status == true)
                .ToListAsync();

            // If there is no active team return with message:
            if (!activeTeams.Any())
            {
                return ApiResponse<List<StandingRowDto>>.Success(new List<StandingRowDto>(), "Henüz aktif takım bulunmuyor.");
            }

            // Map the teams to the dictionary
            var standingsMap = _mapper.Map<List<StandingRowDto>>(activeTeams)
                                  .ToDictionary(x => x.TeamId);

            // Get the completed matches
            var complatedMatches = await _context.Matches
                .Where(m => m.Status.StartsWith("Tamamland") && m.HomeScore.HasValue && m.AwayScore.HasValue)
                .ToListAsync();

            // Calculate matches
            foreach(var match in complatedMatches)
            {
                // Skip match processing if either the home or away team is missing from standings
                if (!standingsMap.TryGetValue(match.HomeTeamId, out var homeTeamRow) || !standingsMap.TryGetValue(match.AwayTeamId, out var awayTeamRow))
                {
                    continue;
                }

                int homeScore = match.HomeScore.Value;
                int awayScore = match.AwayScore.Value;

                homeTeamRow.Played++;
                awayTeamRow.Played++;

                homeTeamRow.GoalsFor += homeScore;
                homeTeamRow.GoalsAgainst += awayScore;

                awayTeamRow.GoalsFor += awayScore;
                awayTeamRow.GoalsAgainst += homeScore;

                if(homeScore > awayScore)
                {
                    homeTeamRow.Won++;
                    homeTeamRow.Points += 3;
                    awayTeamRow.Lost++;
                } else if(homeScore < awayScore)
                {
                    awayTeamRow.Won++;
                    awayTeamRow.Points += 3;
                    homeTeamRow.Lost++;
                } else
                {
                    homeTeamRow.Drawn++;
                    awayTeamRow.Drawn++;
                    homeTeamRow.Points++;
                    awayTeamRow.Points++;
                }
            }

            // Average and ranking
            foreach (var row in standingsMap.Values)
            {
                row.GoalDifference = row.GoalsFor - row.GoalsAgainst;
            }

            var sortedStandings = standingsMap.Values
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ToList();

            // Assign a sequence number
            for (int i = 0; i < sortedStandings.Count; i++)
            {
                sortedStandings[i].Position = i + 1;
            }

            return ApiResponse<List<StandingRowDto>>.Success(sortedStandings, "Puan durumu başarıyla hesaplandı.");
        }
    }
}
