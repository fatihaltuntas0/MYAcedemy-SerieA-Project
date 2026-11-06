using Microsoft.AspNetCore.Mvc;

namespace SerieA.API.Common
{
    public static class ApiResponseExtensions
    {
        public static IActionResult ToActionResult(this ControllerBase c, ApiResponse r)
            => r.IsSuccess ? c.Ok(r)
             : r.IsNotFound ? c.NotFound(r)
             : c.BadRequest(r);

    }
}
