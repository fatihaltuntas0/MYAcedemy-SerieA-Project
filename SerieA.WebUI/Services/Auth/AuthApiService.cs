using SerieA.WebUI.DTOs.Auth;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Services.Auth
{
    public class AuthApiService(HttpClient _client) : IAuthApiService
    {
        public async Task<ApiResponse<LoginResponseDto>?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("auth/login", dto, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>(cancellationToken: cancellationToken);

            return result;
        }
    }
}
