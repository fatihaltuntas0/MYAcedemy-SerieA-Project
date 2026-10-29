using Microsoft.AspNetCore.Mvc;
using SerieA.API.Services.Standings;

namespace SerieA.API.Controllers
{
    [Route("api/standings")]
    [ApiController]
    public class StandingsController(IStandingService _standingService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStandingAsync()
        {
            var result = await _standingService.CalculateStandingAsync();

            return Ok(result);
        }
    }
}
