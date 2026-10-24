using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services
{
    public interface ITeamApiService
    {
        Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q = null, string? region = null, string? status = null);
        Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync();
        Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id);
        Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto);
        Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto);
        Task<ApiResponse> DeleteTeamAsync(Guid id);
    }
}
