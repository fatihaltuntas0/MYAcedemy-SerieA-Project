using SerieA.API.Common;
using SerieA.API.DTOs.Matches.Events;

namespace SerieA.API.Services.MatchEvents
{
    public interface IMatchEventService
    {
        // Goal events
        Task<ApiResponse<Guid>> AddGoalAsync(Guid matchId, CreateGoalDto createGoalDto);
        Task<ApiResponse> DeleteGoalAsync(Guid goalId);

        // Card events
        Task<ApiResponse<Guid>> AddCardAsync(Guid matchId, CreateCardDto createCardDto);
        Task<ApiResponse> DeleteCardAsync(Guid cardId);

        // Substitution events
        Task<ApiResponse<Guid>> AddSubstitutionAsync(Guid matchId, CreateSubstitutionDto createSubstitutionDto);
        Task<ApiResponse> DeleteSubstitutionAsync(Guid subId);
    }
}
