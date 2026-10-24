using SerieA.WebUI.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SerieA.WebUI.Handlers
{
    public class ApiExceptionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken token)
        {
            try
            {
                // Send the request to the API
                var response = await base.SendAsync(message, token);

                // If the API returns an error code such as 500 or 404, and the response is not JSON (but rather HTML or text)
                if (!response.IsSuccessStatusCode)
                {
                    var mediaType = response.Content.Headers.ContentType?.MediaType;
                    var isJson = mediaType != null && mediaType.ToLower().Contains("json");

                    
                    if (!isJson)
                    {
                        var fallbackError = ApiResponse.Fail($"Sunucu hatası oluştu. (HTTP {(int)response.StatusCode})");

                        // Return a valid JSON response in the standard ApiResponse format to the client (TeamApiService)
                        var customResponse = new HttpResponseMessage(response.StatusCode)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(fallbackError), Encoding.UTF8, "application/json")
                        };

                        return customResponse;
                    } 
                } 
                return response;
            } catch (HttpRequestException ex)
            {
                var connectionError = ApiResponse.Fail($"API sunucusuna ulaşılamadı: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(JsonSerializer.Serialize(connectionError), System.Text.Encoding.UTF8, "application/json")
                };
            }

        }
    }
}
