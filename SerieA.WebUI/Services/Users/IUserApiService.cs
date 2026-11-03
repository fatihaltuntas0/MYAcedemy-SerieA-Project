using SerieA.WebUI.DTOs.Users;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Users
{
    public interface IUserApiService
    {
        Task<ApiResponse<List<ResultUserDto>>> GetUsersAsync(string? query = null, UserRole? role = null, bool? status = null, CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultUserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<ResultUserDto>> CreateUserAsync(CreateUserDto createDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUserAsync(Guid id, UpdateUserDto updateDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
