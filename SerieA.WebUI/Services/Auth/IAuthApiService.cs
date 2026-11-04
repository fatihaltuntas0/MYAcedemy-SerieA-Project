using SerieA.WebUI.DTOs.Auth;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Auth
{
    public interface IAuthApiService
    {
        Task<ApiResponse<LoginResponseDto>?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    }
}
