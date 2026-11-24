using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CronogramaCapilar.API.Configuration;
using CronogramaCapilar.API.Handlers;
using CronogramaCapilar.API.OpenApi;
using Microsoft.AspNetCore.Authorization;

namespace CronogramaCapilar.API;

public static class DependencyInjection
{
    private const int AuthRequestsPerMinute = 20;

    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            // Enums trafegam como texto ("Curly"), nunca como número.
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));

            // JSON malformado responde uma mensagem genérica, sem expor nomes de classes internas.
            options.AllowInputFormatterExceptionMessages = false;
        });

        // O gerador do OpenAPI lê estas opções; assim a documentação também mostra os enums como texto.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

        // Toda rota exige login, a não ser que esteja marcada com [AllowAnonymous].
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitPolicies.Auth, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = AuthRequestsPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    }));
        });

        return services;
    }
}
