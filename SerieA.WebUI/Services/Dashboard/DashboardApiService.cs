using SerieA.WebUI.DTOs.Dashboard;
using SerieA.WebUI.Models;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Services.Dashboard
{
    public class DashboardApiService(HttpClient _client) : IDashboardApiService
    {
        public async Task<ApiResponse<ResultDashboardDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync("dashboard", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultDashboardDto>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<ResultDashboardDto>.Fail("Dashboard verileri alınamadı.");
        }
    }
}
