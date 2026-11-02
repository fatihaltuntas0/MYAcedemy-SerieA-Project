using SerieA.WebUI.DTOs.Matches.Events;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.MatchEvents
{
    public interface IMatchEventApiService
    {
        Task<ApiResponse<Guid>> AddGoalAsync(Guid matchId, CreateGoalDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteGoalAsync(Guid goalId, CancellationToken cancellationToken = default);
        Task<ApiResponse<Guid>> AddCardAsync(Guid matchId, CreateCardDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteCardAsync(Guid cardId, CancellationToken cancellationToken = default);
        Task<ApiResponse<Guid>> AddSubstitutionAsync(Guid matchId, CreateSubstitutionDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteSubstitutionAsync(Guid subId, CancellationToken cancellationToken = default);
    }
}
