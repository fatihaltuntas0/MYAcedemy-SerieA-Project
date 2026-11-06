using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Standings;
using SerieA.WebUI.Models;
using System.Collections.Generic;

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
            var response = await _client.GetAsync(url, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ResultStandingRowDto>>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<List<ResultStandingRowDto>>.Fail("Puan durumu alınamadı.");
        }
    }
}
