using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Matches
{
    public interface IMatchApiService
    {
        Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week = null, string? team = null, CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultMatchDto>> GetMatchByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteMatchAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
