using SerieA.WebUI.DTOs.Dashboard;
using SerieA.WebUI.Models;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Services.Dashboard
{
    public interface IDashboardApiService
    {
        Task<ApiResponse<ResultDashboardDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    }
}
