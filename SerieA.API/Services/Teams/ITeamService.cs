using SerieA.API.Common;
using SerieA.API.DTOs.Teams;

namespace SerieA.API.Services.Teams
{
    public interface ITeamService
    {
        Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q, string? region, string? status);
        Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync();
        Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id);
        Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto);
        Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto);
        Task<ApiResponse> DeleteTeamAsync(Guid id);
    }
}
