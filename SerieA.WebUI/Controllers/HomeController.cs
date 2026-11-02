using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Services.Dashboard;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    public class HomeController(IDashboardApiService _dashboardApiService) : Controller
    {
        [HttpGet]
        [Route("")]
        [Route("Admin")]
        [Route("Home")]
        [Route("Home/Index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _dashboardApiService.GetDashboardSummaryAsync(cancellationToken);
            var model = result.IsSuccess && result.Data != null ? result.Data : new DashboardViewModel();
            return View(model);
        }
    }
}
