using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;

namespace SerieA.WebUI.Handlers
{
    public class AuthHeaderHandler(IHttpContextAccessor _httpContextAccessor) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is not null)
            {
                // Retrieve the JWT token stored in the user's session cookie
                var token = await context.GetTokenAsync("access_token");

                // If there is a token, we add "Bearer {token}" to the header of the request sent to the API
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            // API 401 Unauthorized dönerse WebUI oturumunu da derhal sonlandır
            if (response.StatusCode == HttpStatusCode.Unauthorized && context is not null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return response;
        }
    }
}
