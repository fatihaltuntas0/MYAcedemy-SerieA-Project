using SerieA.WebUI.DTOs.Matches.Events;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.MatchEvents
{
    public class MatchEventApiService(HttpClient _client) : IMatchEventApiService
    {
        public async Task<ApiResponse<Guid>> AddCardAsync(Guid matchId, CreateCardDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync($"matches/{matchId}/cards", dto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<Guid>.Fail("Kart eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse<Guid>> AddGoalAsync(Guid matchId, CreateGoalDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync($"matches/{matchId}/goals", dto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<Guid>.Fail("Gol eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse<Guid>> AddSubstitutionAsync(Guid matchId, CreateSubstitutionDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync($"matches/{matchId}/substitutions", dto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<Guid>.Fail("Oyuncu değişikliği eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteCardAsync(Guid cardId, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"matches/cards/{cardId}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

            return result ?? ApiResponse.Fail("Kart silinirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteGoalAsync(Guid goalId, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"matches/goals/{goalId}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

            return result ?? ApiResponse.Fail("Gol silinirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteSubstitutionAsync(Guid subId, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"matches/substitutions/{subId}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

            return result ?? ApiResponse.Fail("Oyuncu değişikliği silinirken bir hata oluştu.");
        }
    }
}
