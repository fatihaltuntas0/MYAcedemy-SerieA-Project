using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Auth;
using SerieA.API.Entities;

namespace SerieA.API.Services.Auth
{
    public class AuthService(ApiContext _context, ITokenService _tokenService, IPasswordHasher<User> _passwordHasher, IMapper _mapper) : IAuthService
    {
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower().Trim(), cancellationToken);
            if(user is null)
            {
                return ApiResponse<LoginResponseDto>.Fail("Geçersiz e-posta veya şifre.");
            }

            if(!user.Status)
            {
                return ApiResponse<LoginResponseDto>.Fail("Hesabınız devre dışı bırakılmıştır.");
            }

            var verificationPass = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(verificationPass == PasswordVerificationResult.Failed)
            {
                return ApiResponse<LoginResponseDto>.Fail("Geçersiz e-posta veya şifre.");
            }

            // Update last login date
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Token generation and DTO matching
            var token = _tokenService.GenerateToken(user);

            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;

            return ApiResponse<LoginResponseDto>.Success(response, "Giriş başarılı.");
        }
    }
}
