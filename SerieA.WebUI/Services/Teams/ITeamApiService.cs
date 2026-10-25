using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Teams
{
    public interface ITeamApiService
    {
        Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q = null, string? region = null, string? status = null, CancellationToken cancellationToken = default);
        Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteTeamAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
