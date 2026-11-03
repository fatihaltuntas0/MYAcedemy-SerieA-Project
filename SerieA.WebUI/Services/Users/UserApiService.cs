using Microsoft.AspNetCore.WebUtilities;
using SerieA.WebUI.DTOs.Users;
using SerieA.WebUI.Enums;
using SerieA.WebUI.Models;
using System.Net.Http.Json;

namespace SerieA.WebUI.Services.Users
{
    public class UserApiService(HttpClient _client) : IUserApiService
    {
        public async Task<ApiResponse<ResultUserDto>> CreateUserAsync(CreateUserDto createDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("users", createDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultUserDto>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<ResultUserDto>.Fail("Kullanıcı eklenemedi.");
        }

        public async Task<ApiResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync($"users/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

            return result ?? ApiResponse.Fail("Kullanıcı silinemedi");
        }

        public async Task<ApiResponse<ResultUserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync($"users/{id}", cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ResultUserDto>>(cancellationToken: cancellationToken);
            return result ?? ApiResponse<ResultUserDto>.Fail("Kullanıcı bulunamadı.");
        }

        public async Task<ApiResponse<List<ResultUserDto>>> GetUsersAsync(string? query = null, UserRole? role = null, bool? status = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, string?>();
            if(!string.IsNullOrWhiteSpace(query))
            {
                queryParams["q"] = query;
            }
            if(role.HasValue)
            {
                queryParams["role"] = role.Value.ToString();
            }
            if(status.HasValue)
            {
                queryParams["active"] = status.Value.ToString();
            }

            var url = QueryHelpers.AddQueryString("users", queryParams);
            var response = await _client.GetAsync(url, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ResultUserDto>>>(cancellationToken: cancellationToken);

            return result ?? ApiResponse<List<ResultUserDto>>.Fail("Kullanıcılar alınamadı.");
        }

        public async Task<ApiResponse> UpdateUserAsync(Guid id, UpdateUserDto updateDto, CancellationToken cancellationToken = default)
        {
            var response = await _client.PutAsJsonAsync($"users/{id}", updateDto, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

            return result ?? ApiResponse.Fail("Kullanıcı güncellenemedi.");
        }
    }
}
