using SerieA.WebUI.Models;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Services.Dashboard
{
    public class DashboardApiService(HttpClient _client) : IDashboardApiService
    {
        public async Task<ApiResponse<DashboardViewModel>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.GetFromJsonAsync<ApiResponse<DashboardViewModel>>("dashboard", cancellationToken);
                return response ?? ApiResponse<DashboardViewModel>.Fail("Dashboard verileri alınamadı.");
            }
            catch (Exception ex)
            {
                return ApiResponse<DashboardViewModel>.Fail($"Dashboard verileri yüklenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}
