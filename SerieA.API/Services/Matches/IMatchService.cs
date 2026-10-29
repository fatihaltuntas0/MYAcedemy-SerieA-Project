using SerieA.API.Common;
using SerieA.API.DTOs.Matches;

namespace SerieA.API.Services.Matches
{
    public interface IMatchService
    {
        Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week, string? team);
        Task<ApiResponse<ResultMatchDetailDto>> GetMatchByIdAsync(Guid id);
        Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto);
        Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto);
        Task<ApiResponse> DeleteMatchAsync(Guid id);
    }
}
