using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.API.Services.Standings;

namespace SerieA.API.Controllers
{
    [Route("api/standings")]
    [ApiController]
    [Authorize]
    public class StandingsController(IStandingService _standingService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStandingAsync([FromQuery] int? upToWeek = null)
        {
            var result = await _standingService.CalculateStandingAsync(upToWeek);

            return Ok(result);
        }
    }
}
