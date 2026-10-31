using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Standings;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Standings
{
    public class StandingApiService(HttpClient _client) : IStandingApiService
    {
        public async Task<ApiResponse<List<ResultStandingRowDto>>> GetStandingAsync(int? upToWeek = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, string?>();
            if(upToWeek.HasValue)
            {
                queryParams[nameof(upToWeek)] = upToWeek.Value.ToString();
            }

            var url = QueryHelpers.AddQueryString("standings", queryParams);
            var response = await _client.GetFromJsonAsync<ApiResponse<List<ResultStandingRowDto>>>(url, cancellationToken);

            return response ?? ApiResponse<List<ResultStandingRowDto>>.Fail("Puan durumu alınamadı.");
        }
    }
}
