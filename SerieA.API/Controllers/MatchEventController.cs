using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.API.Common;
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
        [HttpPost("{id:guid}/goals")]
        public async Task<IActionResult> AddGoalAsync(Guid id, [FromBody] CreateGoalDto dto)
        {
            var result = await _matchEventService.AddGoalAsync(id, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("goals/{goalId:guid}")]
        public async Task<IActionResult> DeleteGoalAsync(Guid goalId)
        {
            var result = await _matchEventService.DeleteGoalAsync(goalId);
            return this.ToActionResult(result);
        }

        // Card endpoints
        [HttpPost("{id:guid}/cards")]
        public async Task<IActionResult> AddCardAsync(Guid id, [FromBody] CreateCardDto dto)
        {
            var result = await _matchEventService.AddCardAsync(id, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("cards/{cardId:guid}")]
        public async Task<IActionResult> DeleteCardAsync(Guid cardId)
        {
            var result = await _matchEventService.DeleteCardAsync(cardId);
            return this.ToActionResult(result);
        }

        // Substitutions endpoints
        [HttpPost("{id:guid}/substitutions")]
        public async Task<IActionResult> AddSubstitutionAsync(Guid id, [FromBody] CreateSubstitutionDto dto)
        {
            var result = await _matchEventService.AddSubstitutionAsync(id, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("substitutions/{subId:guid}")]
        public async Task<IActionResult> DeleteSubstitutionAsync(Guid subId)
        {
            var result = await _matchEventService.DeleteSubstitutionAsync(subId);
            return this.ToActionResult(result);
        }
    }
}
