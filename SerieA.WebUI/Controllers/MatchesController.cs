using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Matches;
using SerieA.WebUI.DTOs.Matches.Events;
using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.MatchEvents;
using SerieA.WebUI.Services.Teams;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    [Authorize]
    public class MatchesController(IMatchApiService _matchApiService, ITeamApiService _teamApiService, IMatchEventApiService _matchEventApiService) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(int? week, string? team, string? status, CancellationToken cancellationToken)
        {
            var matchesTask = _matchApiService.GetMatchesAsync(week, team, cancellationToken);
            var teamsTask = _teamApiService.GetTeamsAsync(status: "active", cancellationToken: cancellationToken);

            await Task.WhenAll(matchesTask, teamsTask);

            var matchesResult = matchesTask.Result;
            var teamsResult = teamsTask.Result;

            var matches = matchesResult.Data ?? new List<ResultMatchDto>();

            // Support status filter on the WebUI layer as well
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
            {
                matches = matches.Where(m => m.Status == parsedStatus).ToList();
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

        [HttpGet]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.GetMatchByIdAsync(id, cancellationToken);
            return Json(result);
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

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id, CancellationToken cancellationToken)
        {
            var result = await _matchApiService.GetMatchByIdAsync(id, cancellationToken);
            if(!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message ?? "Maç bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddGoal(Guid matchId, [FromBody] CreateGoalDto dto, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.AddGoalAsync(matchId, dto, cancellationToken);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGoal(Guid goalId, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.DeleteGoalAsync(goalId, cancellationToken);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddCard(Guid matchId, [FromBody] CreateCardDto dto, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.AddCardAsync(matchId, dto, cancellationToken);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCard(Guid cardId, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.DeleteCardAsync(cardId, cancellationToken);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddSubstitution(Guid matchId, [FromBody] CreateSubstitutionDto dto, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.AddSubstitutionAsync(matchId, dto, cancellationToken);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSubstitution(Guid subId, CancellationToken cancellationToken)
        {
            var result = await _matchEventApiService.DeleteSubstitutionAsync(subId, cancellationToken);
            return Json(result);
        }
    }
}
