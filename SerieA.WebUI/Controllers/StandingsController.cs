using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.Standings;
using SerieA.WebUI.ViewModels;
using System.Threading.Tasks;

namespace SerieA.WebUI.Controllers
{
    public class StandingsController(IStandingApiService _standingApiService, IMatchApiService _matchApiService) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(int? upToWeek, CancellationToken cancellationToken)
        {
            var standingsTask = _standingApiService.GetStandingAsync(upToWeek, cancellationToken);
            var matchesTask = _matchApiService.GetMatchesAsync(cancellationToken: cancellationToken);

            await Task.WhenAll(standingsTask, matchesTask);

            var standingsResult = standingsTask.Result;
            var matchesResult = matchesTask.Result;

            var matches = matchesResult.Data ?? new();

            var availableWeeks = matches
                .Where(m => m.Status == MatchStatus.Finished)
                .Select(m => m.Week)
                .Distinct()
                .OrderBy(w => w)
                .ToList();

            var viewModel = new StandingIndexViewModel
            {
                Standings = standingsResult.Data ?? new(),
                AvailableWeeks = availableWeeks,
                SelectedWeek = upToWeek
            };

            return View(viewModel);
        }
    }
}
