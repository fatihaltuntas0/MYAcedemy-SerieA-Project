using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.API.DTOs.Matches.Events;
using SerieA.API.Services.MatchEvents;
using System.Threading.Tasks;

namespace SerieA.API.Controllers
{
    [Authorize]
    [Route("api/matches")]
    [ApiController]
    public class MatchEventController(IMatchEventService _matchEventService) : ControllerBase
    {

        // Goal endpoints
        [HttpPost("{id}/goals")]
        public async Task<IActionResult> AddGoalAsync(Guid id, [FromBody] CreateGoalDto dto)
        {
            var result = await _matchEventService.AddGoalAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("goals/{goalId}")]
        public async Task<IActionResult> DeleteGoalAsync(Guid goalId)
        {
            var result = await _matchEventService.DeleteGoalAsync(goalId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // Card endpoints
        [HttpPost("{id}/cards")]
        public async Task<IActionResult> AddCardAsync(Guid id, [FromBody] CreateCardDto dto)
        {
            var result = await _matchEventService.AddCardAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("cards/{cardId}")]
        public async Task<IActionResult> DeleteCardAsync(Guid cardId)
        {
            var result = await _matchEventService.DeleteCardAsync(cardId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // Substitutions endpoints
        [HttpPost("{id}/substitutions")]
        public async Task<IActionResult> AddSubstitutionAsync(Guid id, [FromBody] CreateSubstitutionDto dto)
        {
            var result = await _matchEventService.AddSubstitutionAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("substitutions/{subId}")]
        public async Task<IActionResult> DeleteSubstitutionAsync(Guid subId)
        {
            var result = await _matchEventService.DeleteSubstitutionAsync(subId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
