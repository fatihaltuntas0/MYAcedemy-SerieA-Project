using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Common;
using SerieA.API.Context;
using SerieA.API.DTOs.Users;
using SerieA.API.Entities;
using SerieA.API.Enums;

namespace SerieA.API.Services.Users
{
    public class UserService(ApiContext _context, IMapper _mapper, IPasswordHasher<User> _passwordHasher) : IUserService
    {
        public async Task<ApiResponse<ResultUserDto>> CreateUserAsync(CreateUserDto createDto)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == createDto.Email);

            if(emailExists)
            {
                return ApiResponse<ResultUserDto>.Fail("Bu e-posta adresi ile kayıtlı bir kullanıcı zaten mevcut.");
            }

            var user = _mapper.Map<User>(createDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, createDto.Password);
            user.CreatedAt = DateTime.Now;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ResultUserDto>(user);
            return ApiResponse<ResultUserDto>.Success(resultDto, "Kullanıcı başarıyla oluşturuldu.");
        }

        public async Task<ApiResponse> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user is null)
            {
                return ApiResponse.Fail("Silinecek kullanıcı bulunamadı.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return ApiResponse.Success("Kullanıcı başarıyla silindi.");
        }

        public async Task<ApiResponse<ResultUserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if(user is null)
            {
                return ApiResponse<ResultUserDto>.Fail("Kullanıcı bulunamadı.");
            }

            var dtoUser = _mapper.Map<ResultUserDto>(user);
            return ApiResponse<ResultUserDto>.Success(dtoUser);
        }

        public async Task<ApiResponse<List<ResultUserDto>>> GetUsersAsync(string? query = null, UserRole? role = null, bool? status = null)
        {
            var userQuery = _context.Users.AsNoTracking().AsQueryable();
            if(!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                userQuery = userQuery.Where(u => u.Name.ToLower().Contains(q) || u.Email.ToLower().Contains(q));
            }

            if(role.HasValue)
            {
                userQuery = userQuery.Where(u => u.Role == role.Value);
            }

            if(status.HasValue)
            {
                userQuery = userQuery.Where(s => s.Status == status.Value);
            }

            var users = await userQuery.OrderByDescending(u => u.CreatedAt).ToListAsync();
            var dtoList = _mapper.Map<List<ResultUserDto>>(users);

            return ApiResponse<List<ResultUserDto>>.Success(dtoList, "Kullanıcılar başarıyla listelendi.");
        }

        public async Task<ApiResponse> UpdateUserAsync(Guid id, UpdateUserDto updateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user is null)
            {
                return ApiResponse.Fail("Güncellenecek kullanıcı bulunamadı");
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == updateDto.Email.Trim().ToLower());

            if(emailExists)
            {
                return ApiResponse.Fail("Bu eposta adresi başka bir kullanıcı tarafından kullanılıyor.");
            }

            _mapper.Map(updateDto, user);

            if(!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, updateDto.Password);
            }

            await _context.SaveChangesAsync();

            return ApiResponse.Success("Kullanıcı başarıyla güncellendi.");
        }
    }
}
