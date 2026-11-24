using CronogramaCapilar.Domain.Repositories.Interfaces;
using CronogramaCapilar.Infrastructure.Persistence;

namespace CronogramaCapilar.Infrastructure.Repositories;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
