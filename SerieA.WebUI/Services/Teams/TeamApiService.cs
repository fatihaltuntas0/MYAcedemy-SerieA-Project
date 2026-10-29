using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Teams
{
    public class TeamApiService : ITeamApiService
    {

        private readonly HttpClient _client;

        public TeamApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q = null, string? region = null, string? status = null, CancellationToken cancellationToken = default)
        {
            // Collect the parameters into a Dictionary
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrWhiteSpace(q)) queryParams["q"] = q;
            if (!string.IsNullOrWhiteSpace(region)) queryParams["region"] = region;
            if (!string.IsNullOrWhiteSpace(status)) queryParams["status"] = status;

            // Build the URL with QueryHelpers
            var url = QueryHelpers.AddQueryString("teams", queryParams);

            // Send the request with the cancellationToken
            var response = await _client.GetFromJsonAsync<ApiResponse<List<ResultTeamDto>>>(url, cancellationToken);
            return response ?? ApiResponse<List<ResultTeamDto>>.Fail("Sunucudan yanıt alınamadı.");
        }

        public async Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<LeagueSummaryDto>>("teams/summary", cancellationToken);
            return response ?? ApiResponse<LeagueSummaryDto>.Fail("Lig özeti alınamadı!");
        }
        public async Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<ResultTeamDto>>($"teams/{id}", cancellationToken);
            return response ?? ApiResponse<ResultTeamDto>.Fail("Takım getirilemedi.");
        }
        public async Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("teams", createDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultTeamDto>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<ResultTeamDto>.Fail("Takım eklenirken bir hata oluştu.");
        }
        public async Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PutAsJsonAsync($"teams/{id}", updateDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Takım güncellenirken bir hata oluştu.");
        }
        public async Task<ApiResponse> DeleteTeamAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"teams/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
            return result ?? ApiResponse.Fail("Takım silinirken bir sorun oluştu.");
        }
    }
}
