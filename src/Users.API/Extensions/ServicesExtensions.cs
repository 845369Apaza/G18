using System.Reflection;
using Microsoft.OpenApi;
using MiniApi.Data;
using MiniApi.HealthChecks;
using MiniApi.ExceptionHandlers;
using MiniApi.Http;
using MiniApi.Swagger;

namespace MiniApi.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            DapperSetup.Register();

            services.AddSingleton<DatabaseInitializer>();

            // ALUMNOS: acá se registra el repositorio de cada servicio
            services.AddScoped<UserRepository>();

            // ALUMNOS: acá se registran los clientes HTTP hacia otros servicios

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var name = Assembly.GetEntryAssembly()!.GetName().Name!;
                options.SwaggerDoc("v1", new OpenApiInfo { Title = name, Version = "v1" });
                options.OperationFilter<ErrorExamplesFilter>();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{name}.xml"));
            });

            // Orden: primero los específicos, último el genérico.
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<BusinessRuleExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Sin esto, un JSON mal formado devuelve 400 vacío, sin errorCode.
            services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);

            services.AddHttpContextAccessor();
            services.AddTransient<CorrelationIdHandler>();

            services.AddHealthChecks()
                .AddCheck<SqliteHealthCheck>("sqlite-db", tags: ["database"])
                .AddCheck<ApiStatusCheck>("api-status", tags: ["api"]);

            services.AddHealthChecksUI(setup =>
            {
                setup.SetEvaluationTimeInSeconds(600);
                setup.AddHealthCheckEndpoint("MiApi", "/health");
            }).AddInMemoryStorage();
        }
    }
}
