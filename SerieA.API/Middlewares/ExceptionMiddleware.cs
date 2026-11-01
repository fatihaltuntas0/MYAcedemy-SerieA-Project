using SerieA.API.Common;
using System.Net;
using System.Text.Json;

namespace SerieA.API.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger, IHostEnvironment _env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Sunucu tarafında işlenemeyen bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string userMessage = "Sunucu tarafında beklenmeyen bir hata meydana geldi.";
            List<string>? errorDetails = _env.IsDevelopment() ? new List<string> { exception.Message } : null;

            var response = ApiResponse.Fail(userMessage, errorDetails);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
