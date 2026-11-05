using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Auth;
using SerieA.API.Entities;
using SerieA.API.Services.Auth;

namespace SerieA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ApiContext _context, ITokenService _tokenService, IPasswordHasher<User> _passwordHasher, IMapper _mapper) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Find the user with email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower().Trim());
            if(user is null)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Fail("Geçersiz e-posta veya şifre."));
            }

            // Check if the user is active
            if(!user.Status)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Fail("Hesabınız devre dışı bırakılmıştır."));
            }

            // Verify the password
            var verificationPass = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(verificationPass == PasswordVerificationResult.Failed)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Fail("Geçersiz e-posta veya şifre."));
            }

            // Update the last login value
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Generate token
            var token = _tokenService.GenerateToken(user);

            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;

            return Ok(ApiResponse<LoginResponseDto>.Success(response, "Giriş başarılı."));
        }
    }
}
