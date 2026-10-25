using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Matches;
using SerieA.API.Entities;

namespace SerieA.API.Services.Matches
{
    public class MatchService(ApiContext _context, IMapper _mapper) : IMatchService
    {
        public async Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week, string? team)
        {
            var query = _context.Matches
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
        public async Task<ApiResponse<ResultMatchDto>> GetMatchByIdAsync(Guid id)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (match is null)
            {
                return ApiResponse<ResultMatchDto>.Fail("Aradığınız maç bulunamadı.");
            }
            var dtoMatch = _mapper.Map<ResultMatchDto>(match);
            return ApiResponse<ResultMatchDto>.Success(dtoMatch, "Maç başarıyla getirildi.");
        }
        public async Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto)
        {
            // Check to see if the teams have any other games that week 
            bool isTeamBusy = await _context.Matches.AnyAsync(
                m => m.Week == createDto.Week &&
                (m.HomeTeamId == createDto.HomeTeamId ||
                 m.AwayTeamId == createDto.HomeTeamId ||
                 m.HomeTeamId == createDto.AwayTeamId ||
                 m.AwayTeamId == createDto.AwayTeamId)
            );
            if (isTeamBusy)
            {
                return ApiResponse<ResultMatchDto>.Fail(
                    "Maç eklenemedi!",
                    new List<string> { "Seçilen takımlardan en az birinin bu haftada zaten planlanmış bir maçı bulunmaktadır." }
                );
            }
            // If the teams are eligible, add a game
            var match = _mapper.Map<Match>(createDto);
            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();
            var dtoMatch = _mapper.Map<ResultMatchDto>(match);
            return ApiResponse<ResultMatchDto>.Success(dtoMatch, "Maç başarıyla eklendi.");
        }
        public async Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
            if (match is null)
            {
                return ApiResponse.Fail("Güncellenmek istenen maç bulunamadı.");
            }
            bool isTeamBusy = await _context.Matches.AnyAsync(m =>
                m.Id != id && // Exclude your own game
                m.Week == updateDto.Week && // In the same week?
                (m.HomeTeamId == updateDto.HomeTeamId ||
                 m.AwayTeamId == updateDto.HomeTeamId ||
                 m.HomeTeamId == updateDto.AwayTeamId ||
                 m.AwayTeamId == updateDto.AwayTeamId)
            );
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
                return ApiResponse.Fail("Silinmek istenen maç bulunamadı.");
            }
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return ApiResponse.Success("Maç başarıyla silindi.");
        }
    }
}
