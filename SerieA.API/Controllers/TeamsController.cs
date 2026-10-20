using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs;
using SerieA.API.Entities;

namespace SerieA.API.Controllers
{
    [Route("api/admin/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public TeamsController(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync([FromQuery] string? q, [FromQuery] string? region, [FromQuery] string? status)
        {
            var query = _context.Teams.AsQueryable();
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

            return Ok(ApiResponse<List<ResultTeamDto>>.Success(teamDtoList, "Takımlar başarıyla listelendi."));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetLeagueSummaryAsync()
        {
            // The league summary is calculated based only on active teams
            var activeTeams = await _context.Teams
                .Where(t => t.Status == true)
                .ToListAsync();

            if (!activeTeams.Any())
            {
                return Ok(ApiResponse<LeagueSummaryDto>.Success(new LeagueSummaryDto(), "Henüz aktif takım bulunmuyor."));
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

            return Ok(ApiResponse<LeagueSummaryDto>.Success(summaryDto, "Lig özeti başarıyla hesaplandı."));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamByIdAsync(Guid id)
        {
            var teamById = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);

            if (teamById is null)
            {
                return NotFound(ApiResponse<ResultTeamDto>.Fail("Aradığınız takım bulunamadı."));
            }

            var team = _mapper.Map<ResultTeamDto>(teamById);

            return Ok(ApiResponse<ResultTeamDto>.Success(team, "Takım başarıyla getirildi."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamUpsertDto createDto)
        {
            // Check for teams with the same name
            bool isExist = await _context.Teams.AnyAsync(t => t.Name.ToLower() == createDto.Name.ToLower());
            if (isExist)
            {
                return BadRequest(ApiResponse.Fail(message: "Takım eklenemedi!", errors: new List<string> { "Bu isimde bir takım zaten kayıtlı!" }));
            }

            // If the newly added team is marked as active, it cannot exceed the 20 limit
            if (createDto.Status)
            {
                int activeCount = await _context.Teams.CountAsync(t => t.Status == true);
                if (activeCount >= 20)
                {
                    return BadRequest(ApiResponse.Fail(
                        message: "Takım eklenemedi!",
                        errors: new List<string> { "Serie A liginde en fazla 20 aktif takım bulunabilir." }
                    ));
                }
            }


            var team = _mapper.Map<Team>(createDto);
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ResultTeamDto>(team);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<ResultTeamDto>.Success(resultDto, "Takım başarıyla eklendi."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamAsync(Guid id, [FromBody] TeamUpsertDto updateDto)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team is null)
            {
                return NotFound(ApiResponse.Fail(message: "Takım güncellenemedi!", errors: new List<string> { "Güncellenmek istenen takım bulunamadı." }));
            }
            _mapper.Map(updateDto, team);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success("Takım bilgileri başarıyla güncellendi."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamAsync(Guid id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team is null)
            {
                return NotFound(ApiResponse.Fail(message: "Takım silinemedi!", errors: new List<string> { "Silinmek istenen takım bulunamadı." }));
            }

            team.Status = false;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success("Takım başarıyla silindi."));
        }
    }
}
