using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.DTOs.Auth;
using SerieA.WebUI.Extensions;
using SerieA.WebUI.Services.Auth;
using System.Security.Claims;

namespace SerieA.WebUI.Controllers
{
    public class AccountController(IAuthApiService _authApiService) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // If the user is already logged in, redirect to the home page
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Send a request to the API
            var response = await _authApiService.LoginAsync(model);
            if (response == null || !response.IsSuccess || response.Data == null)
            {
                ModelState.AddModelError(string.Empty, response?.Message ?? "Geçersiz e-posta veya şifre.");
                return View(model);
            }

            var authData = response.Data;

            // Identification Card (Claims) to Be Stored in a Cookie
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authData.Id.ToString()),
                new(ClaimTypes.Name, authData.Name),
                new(ClaimTypes.Email, authData.Email),
                new(ClaimTypes.Role, authData.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Store the JWT token in a cookie (AuthHeaderHandler will read it from here)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = JwtHelper.GetExpiration(authData.Token) ?? DateTimeOffset.UtcNow.AddHours(1)
            };

            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = authData.Token }
            });

            // Click the cookie to proceed
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirection
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Logout and delete cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
