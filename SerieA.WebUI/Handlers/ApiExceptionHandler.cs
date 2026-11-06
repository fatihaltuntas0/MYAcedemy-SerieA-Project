using SerieA.WebUI.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SerieA.WebUI.Handlers
{
    public class ApiExceptionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            try
            {
                // Send the request to the API
                var response = await base.SendAsync(request, token);

                if (!response.IsSuccessStatusCode)
                {
                    var rawContent = await response.Content.ReadAsStringAsync(token);
                    var mediaType = response.Content.Headers.ContentType?.MediaType;
                    var isJson = mediaType != null && mediaType.ToLower().Contains("json");

                    if (!isJson || string.IsNullOrWhiteSpace(rawContent))
                    {
                        string errorMessage = response.StatusCode switch
                        {
                            HttpStatusCode.Unauthorized => "Oturum süreniz doldu veya yetkiniz bulunmuyor. Lütfen çıkış yapıp tekrar giriş yapın.",
                            HttpStatusCode.Forbidden => "Bu işlem için yetkiniz bulunmamaktadır.",
                            HttpStatusCode.NotFound => "İstenen kaynak bulunamadı.",
                            _ => $"Sunucu hatası oluştu. (HTTP {(int)response.StatusCode})"
                        };

                        var fallbackError = ApiResponse.Fail(errorMessage);
                        return new HttpResponseMessage(response.StatusCode)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(fallbackError), Encoding.UTF8, "application/json")
                        };
                    }

                    try
                    {
                        using var doc = JsonDocument.Parse(rawContent);
                        var root = doc.RootElement;

                        // If response is already an ApiResponse (has isSuccess property), pass through
                        if (root.TryGetProperty("isSuccess", out _) || root.TryGetProperty("IsSuccess", out _))
                        {
                            return new HttpResponseMessage(response.StatusCode)
                            {
                                Content = new StringContent(rawContent, Encoding.UTF8, "application/json")
                            };
                        }

                        // If it is ASP.NET Core ValidationProblemDetails with an object 'errors'
                        var errorList = new List<string>();
                        string message = "Bir doğrulama hatası oluştu.";

                        if (root.TryGetProperty("title", out var titleProp))
                        {
                            message = titleProp.GetString() ?? message;
                        }

                        if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var prop in errorsProp.EnumerateObject())
                            {
                                if (prop.Value.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var err in prop.Value.EnumerateArray())
                                    {
                                        var str = err.GetString();
                                        if (!string.IsNullOrWhiteSpace(str)) errorList.Add(str);
                                    }
                                }
                            }
                        }

                        var standardizedError = ApiResponse.Fail(message, errorList.Count > 0 ? errorList : null);
                        return new HttpResponseMessage(response.StatusCode)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(standardizedError), Encoding.UTF8, "application/json")
                        };
                    }
                    catch
                    {
                        var fallback = ApiResponse.Fail($"İşlem sırasında bir hata oluştu. (HTTP {(int)response.StatusCode})");
                        return new HttpResponseMessage(response.StatusCode)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(fallback), Encoding.UTF8, "application/json")
                        };
                    }
                } 
                return response;
            } catch (HttpRequestException ex)
            {
                var connectionError = ApiResponse.Fail($"API sunucusuna ulaşılamadı: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(JsonSerializer.Serialize(connectionError), Encoding.UTF8, "application/json")
                };
            }

        }
    }
}
