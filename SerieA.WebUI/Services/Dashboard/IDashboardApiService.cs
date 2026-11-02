using SerieA.WebUI.Models;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Services.Dashboard
{
    public interface IDashboardApiService
    {
        Task<ApiResponse<DashboardViewModel>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    }
}
