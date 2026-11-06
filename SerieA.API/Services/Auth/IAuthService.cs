using SerieA.API.Common;
using SerieA.API.DTOs.Auth;

namespace SerieA.API.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    }
}
