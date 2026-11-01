using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Teams;
using SerieA.API.Entities;

namespace SerieA.API.Services.Teams
{
    public class TeamService(ApiContext _context, IMapper _mapper) : ITeamService
    {
        public async Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q, string? region, string? status)
        {
            var query = _context.Teams.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(t => t.Status == true);
                }
                else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(t => t.Status == false);
                }
            }
            query = query.Where(t =>
                (string.IsNullOrEmpty(q) || t.Name.Contains(q) || t.City.Contains(q) || t.Stadium.Contains(q) || t.Code.Contains(q)) &&
                (string.IsNullOrEmpty(region) || t.Region.ToLower() == region.ToLower()));
            var teamList = await query.ToListAsync();
            var teamDtoList = _mapper.Map<List<ResultTeamDto>>(teamList);
            return ApiResponse<List<ResultTeamDto>>.Success(teamDtoList, "Takımlar başarıyla listelendi.");
        }
        public async Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync()
        {
            // The league summary is calculated based only on active teams
            var activeTeams = await _context.Teams
                .AsNoTracking()
                .Where(t => t.Status == true)
                .ToListAsync();

            if (!activeTeams.Any())
            {
                return ApiResponse<LeagueSummaryDto>.Success(new LeagueSummaryDto(), "Henüz aktif takım bulunmuyor.");
            }

            // Deduplication on a stadium-by-stadium basis to count shared stadiums (e.g., San Siro / Giuseppe Meazza) as a single entry
            var totalCapacity = activeTeams
                .GroupBy(t => t.Stadium.ToLower().Trim())
                .Sum(g => g.First().StadiumCapacity);

            // Find the oldest (most established) club
            var oldestClub = activeTeams
                .OrderBy(t => t.FoundedYear)
                .First();

            var summaryDto = new LeagueSummaryDto
            {
                ActiveTeamCount = activeTeams.Count,
                MaxTeamLimit = 20,
                CityCount = activeTeams.Select(t => t.City.ToLower().Trim()).Distinct().Count(),
                TotalStadiumCapacity = totalCapacity,
                OldestClubName = oldestClub.Name,
                OldestClubFoundedYear = oldestClub.FoundedYear.Year
            };

            return ApiResponse<LeagueSummaryDto>.Success(summaryDto, "Lig özeti başarıyla hesaplandı.");
        }

        public async Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id)
        {
            var team = await _context.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (team is null)
            {
                return ApiResponse<ResultTeamDto>.Fail("Aradığınız takım bulunamadı.");
            }

            var result = _mapper.Map<ResultTeamDto>(team);
            return ApiResponse<ResultTeamDto>.Success(result, "Takım başarıyla getirildi.");
        }

        public async Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto)
        {
            // Check for teams with the same name
            bool isExist = await _context.Teams.AnyAsync(t => t.Name.ToLower() == createDto.Name.ToLower());
            if (isExist)
            {
                return ApiResponse<ResultTeamDto>.Fail("Takım eklenemedi!", new List<string> { "Bu isimde bir takım zaten kayıtlı!" });
            }

            // If the newly added team is marked as active, it cannot exceed the 20 limit
            if (createDto.Status)
            {
                int activeCount = await _context.Teams.CountAsync(t => t.Status == true);
                if (activeCount >= 20)
                {
                    return ApiResponse<ResultTeamDto>.Fail(
                        "Takım eklenemedi!",
                        new List<string> { "Serie A liginde en fazla 20 aktif takım bulunabilir." }
                    );
                }
            }

            var team = _mapper.Map<Team>(createDto);
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ResultTeamDto>(team);
            return ApiResponse<ResultTeamDto>.Success(resultDto, "Takım başarıyla eklendi.");
        }
        public async Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team is null)
            {
                return ApiResponse.Fail("Takım güncellenemedi!", new List<string> { "Güncellenmek istenen takım bulunamadı." });
            }
            _mapper.Map(updateDto, team);
            await _context.SaveChangesAsync();
            return ApiResponse.Success("Takım bilgileri başarıyla güncellendi.");
        }
        public async Task<ApiResponse> DeleteTeamAsync(Guid id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team is null)
            {
                return ApiResponse.Fail("Takım silinemedi!", new List<string> { "Silinmek istenen takım bulunamadı." });
            }
            team.Status = false;
            await _context.SaveChangesAsync();
            return ApiResponse.Success("Takım başarıyla silindi.");
        }
        
    }
}
