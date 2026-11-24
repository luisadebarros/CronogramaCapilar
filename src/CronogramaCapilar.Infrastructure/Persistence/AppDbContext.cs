using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace CronogramaCapilar.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>Cada tabela tem sua própria classe de configuração na pasta Configurations.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // O SQLite não guarda o "tipo" da data; isto garante que toda data lida volte marcada como UTC.
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }
}
