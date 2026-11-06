using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Users;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Models;
using SerieA.WebUI.Services.Users;
using SerieA.WebUI.ViewModels;
using System.Threading.Tasks;

namespace SerieA.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController(IUserApiService _userApiService) : Controller
    {
        // Main page and filtering
        [HttpGet]
        [Route("Users")]
        [Route("Admin/Users")]
        public async Task<IActionResult> Index(string? q, UserRole? role, bool? active, CancellationToken cancellationToken)
        {
            var userResult = await _userApiService.GetUsersAsync(q, role, active, cancellationToken);

            var viewModel = new UserIndexViewModel
            {
                Users = userResult.Data ?? new List<ResultUserDto>(),
                Query = q,
                Role = role,
                Active = active
            };

            return View(viewModel);
        }

        // Retrieve a single user when the edit modal opens (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userApiService.GetUserByIdAsync(id, cancellationToken);
            return Json(result);
        }

        // Add new user (AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Json(ApiResponse.Fail("Lütfen form verilerini eksiksiz giriniz."));
            }

            var result = await _userApiService.CreateUserAsync(createUserDto, cancellationToken);
            return Json(result);
        }

        // Update user (AJAX)
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Json(ApiResponse.Fail("Lütfen form verilerini eksiksiz giriniz."));
            }

            var result = await _userApiService.UpdateUserAsync(id, updateUserDto, cancellationToken);
            return Json(result);
        }

        // Delete user (AJAX)
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userApiService.DeleteUserAsync(id, cancellationToken);
            return Json(result);
        }
    }
}
