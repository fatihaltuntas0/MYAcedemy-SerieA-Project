using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    public class WeeksController(IMatchApiService _matchApiService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _matchApiService.GetWeeksSummaryAsync(cancellationToken);
            var viewModel = new WeekIndexViewModel
            {
                Weeks = result.Data ?? new()
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetWeekMatches(int week, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.GetMatchesAsync(week: week, cancellationToken: cancellationToken);
            return Json(result);
        }
    }
}
