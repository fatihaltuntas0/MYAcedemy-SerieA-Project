using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Models;
using System.Collections.Generic;

namespace SerieA.WebUI.Services.Matches
{
    public class MatchApiService(HttpClient _client) : IMatchApiService
    {
        public async Task<ApiResponse<List<ResultMatchDto>>> GetMatchesAsync(int? week = null, string? team = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, string?>();
            if (week.HasValue) queryParams["week"] = week.Value.ToString();
            if (!string.IsNullOrWhiteSpace(team)) queryParams["team"] = team;

            var url = QueryHelpers.AddQueryString("matches", queryParams);
            var response = await _client.GetAsync(url, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ResultMatchDto>>>(cancellationToken:cancellationToken);
            return result ?? ApiResponse<List<ResultMatchDto>>.Fail("Maçlar alınamadı.");
        }

        public async Task<ApiResponse<ResultMatchDetailDto>> GetMatchByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync($"matches/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultMatchDetailDto>>(cancellationToken:cancellationToken);
            return result ?? ApiResponse<ResultMatchDetailDto>.Fail("Maç alınamadı.");
        }

        public async Task<ApiResponse<ResultMatchDto>> CreateMatchAsync(MatchUpsertDto createDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("matches", createDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultMatchDto>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<ResultMatchDto>.Fail("Maç eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> UpdateMatchAsync(Guid id, MatchUpsertDto updateDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PutAsJsonAsync($"matches/{id}", updateDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Maç güncellenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteMatchAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"matches/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Maç silinirken bir hata oluştu.");
        }

        public async Task<ApiResponse<List<ResultWeekSummaryDto>>> GetWeeksSummaryAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync("matches/weeks-summary", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ResultWeekSummaryDto>>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<List<ResultWeekSummaryDto>>.Fail("Haftalık özet bilgileri alınamadı.");
        }
    }
}
