using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.API.DTOs.Matches;
using SerieA.API.Services.Matches;
using System.Threading.Tasks;

namespace SerieA.API.Controllers
{
    [Authorize]
    [Route("api/matches")]
    [ApiController]
    public class MatchesController(IMatchService _matchService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMatchesAsync([FromQuery] int? week = null, [FromQuery] string? team = null)
        {
            var result = await _matchService.GetMatchesAsync(week, team);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("week/{week:int}")]
        public async Task<IActionResult> GetMatchesByWeekAsync(int week)
        {
            var result = await _matchService.GetMatchesAsync(week, null);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("weeks-summary")]
        public async Task<IActionResult> GetWeekSummaryAsync()
        {
            var result = await _matchService.GetWeekSummaryAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchByIdAsync(Guid id)
        {
            var result = await _matchService.GetMatchByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMatchAsync([FromBody] MatchUpsertDto createDto)
        {
            var result = await _matchService.CreateMatchAsync(createDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status201Created, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatchAsync(Guid id, [FromBody] MatchUpsertDto updateDto)
        {
            var result = await _matchService.UpdateMatchAsync(id, updateDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatchAsync(Guid id)
        {
            var result = await _matchService.DeleteMatchAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
