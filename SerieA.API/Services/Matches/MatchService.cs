using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Matches;
using SerieA.API.DTOs.Standings;
using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.Services.Matches
{
    public class MatchService(ApiContext _context, IMapper _mapper) : IMatchService
    {
        public async Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week, string? team)
        {
            var query = _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsQueryable();

            if (week.HasValue)
            {
                query = query.Where(m => m.Week == week.Value);
            }
            if (!string.IsNullOrWhiteSpace(team))
            {
                query = query.Where(m => m.HomeTeam.Name.Contains(team) || m.AwayTeam.Name.Contains(team));
            }

            var list = await query.OrderBy(m => m.Week).ThenBy(m => m.MatchDate).ToListAsync();
            var dtoList = _mapper.Map<List<ResultMatchDto>>(list);
            return ApiResponse<List<ResultMatchDto>>.Success(dtoList, "Maçlar başarıyla listelendi.");
        }

        public async Task<ApiResponse<ResultMatchDetailDto>> GetMatchByIdAsync(Guid id)
        {
            var match = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.MatchGoals).ThenInclude(g => g.Team)
                .Include(m => m.MatchCards).ThenInclude(c => c.Team)
                .Include(m => m.Substitutions).ThenInclude(s => s.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match is null)
            {
                return ApiResponse<ResultMatchDetailDto>.NotFound("Aradığınız maç bulunamadı.");
            }

            var detail = _mapper.Map<ResultMatchDetailDto>(match);

            return ApiResponse<ResultMatchDetailDto>.Success(detail, "Maç başarıyla getirildi.");
        }

        public async Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto)
        {
            bool isTeamBusy = await IsTeamBusyAsync(createDto.Week, createDto.HomeTeamId, createDto.AwayTeamId);
            if (isTeamBusy)
            {
                return ApiResponse<ResultMatchDto>.Fail(
                    "Maç eklenemedi!",
                    new List<string> { "Seçilen takımlardan en az birinin bu haftada zaten planlanmış bir maçı bulunmaktadır." }
                );
            }

            var match = _mapper.Map<Match>(createDto);
            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();

            await _context.Entry(match).Reference(m => m.HomeTeam).LoadAsync();
            await _context.Entry(match).Reference(m => m.AwayTeam).LoadAsync();

            var dtoMatch = _mapper.Map<ResultMatchDto>(match);
            return ApiResponse<ResultMatchDto>.Success(dtoMatch, "Maç başarıyla eklendi.");
        }

        public async Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
            if (match is null)
            {
                return ApiResponse.NotFound("Güncellenmek istenen maç bulunamadı.");
            }

            bool isTeamBusy = await IsTeamBusyAsync(updateDto.Week, updateDto.HomeTeamId, updateDto.AwayTeamId, excludeMatchId: id);
            if (isTeamBusy)
            {
                return ApiResponse.Fail(
                    "Maç güncellenemedi!",
                    new List<string> { "Seçilen takımlardan en az birinin bu haftada zaten planlanmış bir maçı bulunmaktadır." }
                );
            }

            _mapper.Map(updateDto, match);
            await _context.SaveChangesAsync();
            return ApiResponse.Success("Maç bilgileri güncellendi.");
        }

        public async Task<ApiResponse> DeleteMatchAsync(Guid id)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
            if (match is null)
            {
                return ApiResponse.NotFound("Silinmek istenen maç bulunamadı.");
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return ApiResponse.Success("Maç başarıyla silindi.");
        }

        public async Task<ApiResponse<List<WeekSummaryDto>>> GetWeekSummaryAsync()
        {
            var allMatches = await _context.Matches.AsNoTracking().ToListAsync();

            if (!allMatches.Any())
            {
                return ApiResponse<List<WeekSummaryDto>>.Success(new List<WeekSummaryDto>(), "Henüz planlanmış bir maç veya hafta bulunmuyor.");
            }

            var summaryList = allMatches
                .GroupBy(m => m.Week)
                .OrderBy(g => g.Key)
                .Select(group => BuildWeekSummary(group.Key, group.ToList()))
                .ToList();

            return ApiResponse<List<WeekSummaryDto>>.Success(summaryList, "Haftalık özet başarıyla hesaplandı.");
        }

        #region Private Helpers - Weekly Summary & Match Validation

        private WeekSummaryDto BuildWeekSummary(int week, List<Match> matches)
        {
            var dates = matches.Select(m => m.MatchDate).OrderBy(d => d).ToList();
            var completedCount = matches.Count(m => m.Status == MatchStatus.Finished);
            var uniqueTeamsCount = GetUniqueTeamsCount(matches);

            return new WeekSummaryDto
            {
                Week = week,
                StartDate = dates.First(),
                EndDate = dates.Last(),
                TotalMatches = matches.Count,
                CompletedMatches = completedCount,
                Status = DetermineWeekStatus(matches.Count, completedCount, matches),
                IsValidFixture = ValidateFixture(matches.Count, uniqueTeamsCount),
                UniqueTeamsCount = uniqueTeamsCount
            };
        }

        private MatchStatus DetermineWeekStatus(int total, int completed, List<Match> matches)
        {
            if (completed == total && total > 0)
            {
                return MatchStatus.Finished;
            }
            if (completed > 0 || matches.Any(m => m.Status == MatchStatus.Live))
            {
                return MatchStatus.Live;
            }
            return MatchStatus.NotStarted;
        }

        private bool ValidateFixture(int totalMatches, int uniqueTeamsCount) => totalMatches == 10 && uniqueTeamsCount == 20;

        private int GetUniqueTeamsCount(List<Match> matches) =>
            matches.Select(m => m.HomeTeamId)
                   .Concat(matches.Select(m => m.AwayTeamId))
                   .Distinct()
                   .Count();

        private async Task<bool> IsTeamBusyAsync(int week, Guid homeTeamId, Guid awayTeamId, Guid? excludeMatchId = null)
        {
            return await _context.Matches.AsNoTracking().AnyAsync(m =>
                (!excludeMatchId.HasValue || m.Id != excludeMatchId.Value) &&
                m.Week == week &&
                (m.HomeTeamId == homeTeamId ||
                 m.AwayTeamId == homeTeamId ||
                 m.HomeTeamId == awayTeamId ||
                 m.AwayTeamId == awayTeamId)
            );
        }

        #endregion
    }
}
