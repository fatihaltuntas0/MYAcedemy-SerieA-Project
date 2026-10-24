using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services
{
    public class TeamApiService : ITeamApiService
    {

        private readonly HttpClient _client;

        public TeamApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<ResultTeamDto>> CreateTeamAsync(TeamUpsertDto createDto)
        {
            var response = await _client.PostAsJsonAsync("admin/teams", createDto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultTeamDto>>();

            return result ?? ApiResponse<ResultTeamDto>.Fail("Takım eklenirken bir hata oluştu.");
        }

        public async Task<ApiResponse> DeleteTeamAsync(Guid id)
        {
            var response = await _client.DeleteAsync($"admin/teams/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return result ?? ApiResponse.Fail("Takım silinirken bir sorun oluştu.");
        }

        public async Task<ApiResponse<LeagueSummaryDto>> GetLeagueSummaryAsync()
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<LeagueSummaryDto>>("admin/teams/summary");
            return response ?? ApiResponse<LeagueSummaryDto>.Fail("Lig özeti alınamadı!");
        }

        public async Task<ApiResponse<List<ResultTeamDto>>> GetTeamsAsync(string? q = null, string? region = null, string? status = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(q)) queryParams.Add($"q={Uri.EscapeDataString(q)}");
            if (!string.IsNullOrWhiteSpace(region)) queryParams.Add($"region={Uri.EscapeDataString(region)}");
            if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

            var response = await _client.GetFromJsonAsync<ApiResponse<List<ResultTeamDto>>>($"admin/teams{queryString}");
            return response ?? ApiResponse<List<ResultTeamDto>>.Fail("Sunucudan yanıt alınamadı.");
        }

        public async Task<ApiResponse<ResultTeamDto>> GetTeamByIdAsync(Guid id)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<ResultTeamDto>>($"admin/teams/{id}");

            return response ?? ApiResponse<ResultTeamDto>.Fail("Takım getirilemedi.");
        }

        public async Task<ApiResponse> UpdateTeamAsync(Guid id, TeamUpsertDto updateDto)
        {
            var response = await _client.PutAsJsonAsync($"admin/teams/{id}", updateDto);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return result ?? ApiResponse.Fail("Takım güncellenirken bir hata oluştu.");
        }
    }
}
