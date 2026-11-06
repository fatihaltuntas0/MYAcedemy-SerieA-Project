using SerieA.WebUI.Models;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Services.Dashboard
{
    public class DashboardApiService(HttpClient _client) : IDashboardApiService
    {
        public async Task<ApiResponse<DashboardViewModel>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync("dashboard", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardViewModel>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<DashboardViewModel>.Fail("Dashboard verileri alınamadı.");
        }
    }
}
