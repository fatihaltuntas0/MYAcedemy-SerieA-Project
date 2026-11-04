using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.API.DTOs.Teams;
using SerieA.API.Services.Teams;

namespace SerieA.API.Controllers
{
    [Authorize]
    [Route("api/teams")]
    [ApiController]
    public class TeamsController(ITeamService _teamService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync([FromQuery] string? q, [FromQuery] string? region, [FromQuery] string? status)
        {
            var result = await _teamService.GetTeamsAsync(q, region, status);
            return Ok(result);
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetLeagueSummaryAsync()
        {
            var result = await _teamService.GetLeagueSummaryAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamByIdAsync(Guid id)
        {
            var result = await _teamService.GetTeamByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamUpsertDto createDto)
        {
            var result = await _teamService.CreateTeamAsync(createDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status201Created, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeamAsync(Guid id, [FromBody] TeamUpsertDto updateDto)
        {
            var result = await _teamService.UpdateTeamAsync(id, updateDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamAsync(Guid id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
