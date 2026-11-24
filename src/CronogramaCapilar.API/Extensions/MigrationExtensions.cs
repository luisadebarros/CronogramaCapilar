using CronogramaCapilar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CronogramaCapilar.API.Extensions;

public static class MigrationExtensions
{
    /// <summary>Cria ou atualiza o banco ao subir a API. Usado só em desenvolvimento.</summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
