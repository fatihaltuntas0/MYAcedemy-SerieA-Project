using SerieA.API.Common;
using SerieA.API.DTOs.Users;
using SerieA.API.Enums;

namespace SerieA.API.Services.Users
{
    public interface IUserService
    {
        Task<ApiResponse<List<ResultUserDto>>> GetUsersAsync(string? query = null, UserRole? role = null, bool? status = null);
        Task<ApiResponse<ResultUserDto>> GetUserByIdAsync(Guid id);
        Task<ApiResponse<ResultUserDto>> CreateUserAsync(CreateUserDto createDto);
        Task<ApiResponse> UpdateUserAsync(Guid id, UpdateUserDto updateDto);
        Task<ApiResponse> DeleteUserAsync(Guid id);
    }
}
