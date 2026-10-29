using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Matches
{
    public class MatchApiService(HttpClient _client) : IMatchApiService
    {
        public async Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week = null, string? team = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, string?>();
            if (week.HasValue) queryParams["week"] = week.Value.ToString();
            if (!string.IsNullOrWhiteSpace(team)) queryParams["team"] = team;

            var url = QueryHelpers.AddQueryString("admin/matches", queryParams);
            var response = await _client.GetFromJsonAsync<ApiResponse<List<ResultMatchDto>>>(url, cancellationToken);
            return response ?? ApiResponse<List<ResultMatchDto>>.Fail("Maçlar alınamadı.");
        }

        public async Task<ApiResponse<ResultMatchDetailDto>> GetMatchByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<ResultMatchDetailDto>>($"admin/matches/{id}", cancellationToken);
            return response ?? ApiResponse<ResultMatchDetailDto>.Fail("Maç bulunamadı.");
        }

        public async Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("admin/matches", createDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultMatchDto>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<ResultMatchDto>.Fail("Maç eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PutAsJsonAsync($"admin/matches/{id}", updateDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Maç güncellenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteMatchAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"admin/matches/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Maç silinirken bir hata oluştu.");
        }
    }
}
