using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Dashboard;
using SerieA.API.DTOs.Matches;
using SerieA.API.DTOs.Standings;
using SerieA.API.Enums;
using SerieA.API.Services.Standings;

namespace SerieA.API.Services.Dashboard
{
    public class DashboardService(
        ApiContext _context,
        IStandingService _standingService,
        IMapper _mapper) : IDashboardService
    {
        public async Task<ApiResponse<ResultDashboardDto>> GetDashboardSummaryAsync()
        {
            var activeTeamCount = await _context.Teams.CountAsync(t => t.Status == true);
            var finishedMatchCount = await _context.Matches.CountAsync(m => m.Status == MatchStatus.Finished);
            var totalMatchCount = await _context.Matches.CountAsync();

            var totalGoals = await _context.Matches
                .Where(m => m.Status == MatchStatus.Finished && m.HomeScore.HasValue && m.AwayScore.HasValue)
                .SumAsync(m => m.HomeScore!.Value + m.AwayScore!.Value);

            var avgGoals = finishedMatchCount > 0
                ? Math.Round((double)totalGoals / finishedMatchCount, 2)
                : 0.0;

            var nextWeekMatch = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Status != MatchStatus.Finished)
                .OrderBy(m => m.Week)
                .Select(m => (int?)m.Week)
                .FirstOrDefaultAsync();

            var maxWeek = await _context.Matches.MaxAsync(m => (int?)m.Week) ?? 1;
            var nextWeek = nextWeekMatch ?? maxWeek;

            // 6 most recent completed matches
            var recentMatchesEntities = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status == MatchStatus.Finished)
                .OrderByDescending(m => m.MatchDate)
                .ThenByDescending(m => m.Week)
                .Take(6)
                .ToListAsync();

            // 5 upcoming scheduled/live matches
            var upcomingMatchesEntities = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status != MatchStatus.Finished)
                .OrderBy(m => m.MatchDate)
                .ThenBy(m => m.Week)
                .Take(5)
                .ToListAsync();

            // Top 5 from calculated standings
            var standingsResult = await _standingService.CalculateStandingAsync();
            var topStandings = standingsResult.IsSuccess && standingsResult.Data != null
                ? standingsResult.Data.Take(5).ToList()
                : new List<StandingRowDto>();

            var dashboardDto = new ResultDashboardDto
            {
                ActiveTeamCount = activeTeamCount,
                FinishedMatchCount = finishedMatchCount,
                TotalMatchCount = totalMatchCount,
                TotalGoals = totalGoals,
                AverageGoalsPerMatch = avgGoals,
                NextWeek = nextWeek,
                RecentMatches = _mapper.Map<List<ResultMatchDto>>(recentMatchesEntities),
                UpcomingMatches = _mapper.Map<List<ResultMatchDto>>(upcomingMatchesEntities),
                TopStandings = topStandings
            };

            return ApiResponse<ResultDashboardDto>.Success(dashboardDto, "Dashboard verileri başarıyla getirildi.");
        }
    }
}
