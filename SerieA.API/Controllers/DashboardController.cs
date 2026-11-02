using Microsoft.AspNetCore.Mvc;
using SerieA.API.Services.Dashboard;

namespace SerieA.API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(IDashboardService _dashboardService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummaryAsync()
        {
            var result = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(result);
        }
    }
}
