using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.Teams;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IMatchApiService _matchApiService;
        private readonly ITeamApiService _teamApiService;

        public MatchesController(IMatchApiService matchApiService, ITeamApiService teamApiService)
        {
            _matchApiService = matchApiService;
            _teamApiService = teamApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? week, string? team, string? status, CancellationToken cancellationToken)
        {
            var matchesTask = _matchApiService.GetMatchesAsync(week, team, cancellationToken);
            var teamsTask = _teamApiService.GetTeamsAsync(status: "active", cancellationToken: cancellationToken);

            await Task.WhenAll(matchesTask, teamsTask);

            var matchesResult = matchesTask.Result;
            var teamsResult = teamsTask.Result;

            var matches = matchesResult.Data ?? new List<ResultMatchDto>();

            // Statü filtresi WebUI tarafında da desteklensin
            if (!string.IsNullOrWhiteSpace(status))
            {
                matches = matches.Where(m => m.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var viewModel = new MatchIndexViewModel
            {
                Matches = matches,
                Teams = teamsResult.Data ?? new List<ResultTeamDto>(),
                SelectedWeek = week,
                SearchTeam = team,
                SelectedStatus = status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MatchUpsertDto createDto, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.CreateMatchAsync(createDto, cancellationToken);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, [FromBody] MatchUpsertDto updateDto, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.UpdateMatchAsync(id, updateDto, cancellationToken);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.DeleteMatchAsync(id, cancellationToken);
            return Json(result);
        }
    }
}
