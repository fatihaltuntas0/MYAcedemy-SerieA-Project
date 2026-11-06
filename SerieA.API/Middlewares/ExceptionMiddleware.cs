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
            // 
            if(context.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt akışı önceden başladığı için hata yanıtı yazılamadı.");
                return;
            }

            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            string userMessage = statusCode == HttpStatusCode.InternalServerError ? "Sunucu tarafında beklenmeyen bir hata meydana geldi." : exception.Message;

            List<string>? errorDetails = _env.IsDevelopment() ? new List<string> { exception.Message } : null;

            var response = ApiResponse.Fail(userMessage, errorDetails);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
