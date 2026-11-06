using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Extensions;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.Standings;
using SerieA.WebUI.ViewModels;
using System.Threading.Tasks;

namespace SerieA.WebUI.Controllers
{
    public class PublicController(IStandingApiService _standingApiService, IMatchApiService _matchApiService) : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        [Route("Puan-Durumu")]
        [Route("Public/Standings")]
        public async Task<IActionResult> Standings(int? upToWeek, CancellationToken cancellationToken)
        {
            var standingsTask = _standingApiService.GetStandingAsync(upToWeek, cancellationToken);
            var matchesTask = _matchApiService.GetMatchesAsync(cancellationToken: cancellationToken);

            await Task.WhenAll(standingsTask, matchesTask);

            var standingsResult = standingsTask.Result;
            var matchesResult = matchesTask.Result;

            var availableWeeks = (matchesResult.Data ?? new())
                .Where(m => m.Status == MatchStatus.Finished)
                .Select(w => w.Week)
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

        [AllowAnonymous]
        [HttpGet]
        [Route("Fikstur")]
        [Route("Public/Fixtures")]
        public async Task<IActionResult> Fixtures(int? week, CancellationToken cancellationToken)
        {
            var weeksSummaryTask = _matchApiService.GetWeeksSummaryAsync(cancellationToken);
            var matchesTask = week.HasValue
                ? _matchApiService.GetMatchesAsync(week: week.Value, cancellationToken: cancellationToken)
                : null;

            if (matchesTask != null)
            {
                await Task.WhenAll(weeksSummaryTask, matchesTask);
            }
            else
            {
                await weeksSummaryTask;
            }

            var weeksSummary = weeksSummaryTask.Result.Data ?? new();

            var availableWeeks = weeksSummary.Any()
                ? weeksSummary.Select(w => w.Week).OrderBy(w => w).ToList()
                : Enumerable.Range(1, 38).ToList();

            // If no week has been selected: select the week that hasn't ended yet (currently in progress or next); if all weeks have ended, select the last week
            int selectedWeek = week ??
                weeksSummary.FirstOrDefault(w => w.Status != MatchStatus.Finished)?.Week ??
                availableWeeks.LastOrDefault(1);

            List<ResultMatchDto> matches;
            if (matchesTask != null && selectedWeek == week)
            {
                matches = matchesTask.Result.Data ?? new();
            }
            else
            {
                var matchesResult = await _matchApiService.GetMatchesAsync(week: selectedWeek, cancellationToken: cancellationToken);
                matches = matchesResult.Data ?? new();
            }

            var viewModel = new FixtureIndexViewModel
            {
                SelectedWeek = selectedWeek,
                AvailableWeeks = availableWeeks,
                Matches = matches,
                WeeksSummary = weeksSummary
            };

            return View(viewModel);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("Mac/{id:guid}")]
        [Route("Public/Match/{id:guid}")]
        public async Task<IActionResult> MatchDetail(Guid id, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.GetMatchByIdAsync(id, cancellationToken);
            if(!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var viewModel = result.Data.ToViewModel();

            return View(viewModel);
        }
    }
}
