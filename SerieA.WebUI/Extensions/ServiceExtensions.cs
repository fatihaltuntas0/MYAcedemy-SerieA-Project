using SerieA.WebUI.Handlers;

namespace SerieA.WebUI.Extensions
{
    public static class ServiceExtensions
    {
        public static IHttpClientBuilder AddCustomHttpClient<TInterface, TImplementation>(this IServiceCollection services)
                where TInterface : class
                where TImplementation : class, TInterface
        {
            return services.AddHttpClient<TInterface, TImplementation>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7283/api/";
                client.BaseAddress = new Uri(baseUrl);
            })
                .AddHttpMessageHandler<ApiExceptionHandler>();
        }
    }
}
