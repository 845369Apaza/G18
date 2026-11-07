namespace MiniApi.Http
{
    // Copia el X-Correlation-Id de la request actual en cada llamada a otro servicio.
    public class CorrelationIdHandler(IHttpContextAccessor accessor) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = accessor.HttpContext?.TraceIdentifier;
            if (id is not null)
                request.Headers.TryAddWithoutValidation("X-Correlation-Id", id);

            return base.SendAsync(request, cancellationToken);
        }
    }

    public static class ServiceClientExtensions
    {
        // Uso: services.AddServiceClient<ProductsClient>("Products");
        // Toma la URL de la sección "Services" de appsettings.json.
        public static void AddServiceClient<TClient>(this IServiceCollection services, string serviceName)
            where TClient : class
        {
            services.AddHttpClient<TClient>((provider, client) =>
            {
                var url = provider.GetRequiredService<IConfiguration>()[$"Services:{serviceName}"];
                client.BaseAddress = new Uri(url!);
                client.Timeout = TimeSpan.FromSeconds(10);
            }).AddHttpMessageHandler<CorrelationIdHandler>();
        }
    }
}