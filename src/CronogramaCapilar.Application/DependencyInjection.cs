using CronogramaCapilar.Application.Services;
using CronogramaCapilar.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CronogramaCapilar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
