using SerieA.API.Common;
using SerieA.API.DTOs.Dashboard;

namespace SerieA.API.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<ResultDashboardDto>> GetDashboardSummaryAsync();
    }
}
