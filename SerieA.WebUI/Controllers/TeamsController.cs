using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Teams;
using SerieA.WebUI.Services.Teams;
using SerieA.WebUI.ViewModels;

namespace SerieA.WebUI.Controllers
{
    [Authorize]
    public class TeamsController(ITeamApiService _teamApiService) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? region, string? status)
        {
            var teamsTask = _teamApiService.GetTeamsAsync(q, region, status);
            var summaryTask = _teamApiService.GetLeagueSummaryAsync();

            await Task.WhenAll(teamsTask, summaryTask);

            var teamsResult = teamsTask.Result;
            var summaryResult = summaryTask.Result;

            var viewModel = new TeamIndexViewModel
            {
                Teams = teamsResult.Data ?? new List<ResultTeamDto>(),
                Summary = summaryResult.Data ?? new LeagueSummaryDto(),
                SearchQuery = q,
                SelectedRegion = region,
                SelectedStatus = status
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeamUpsertDto createDto)
        {
            var result = await _teamApiService.CreateTeamAsync(createDto);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, [FromBody] TeamUpsertDto createDto)
        {
            var result = await _teamApiService.UpdateTeamAsync(id, createDto);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _teamApiService.DeleteTeamAsync(id);
            return Json(result);
        }

    }
}
