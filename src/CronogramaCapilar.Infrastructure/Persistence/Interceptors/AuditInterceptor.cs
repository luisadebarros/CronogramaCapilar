using CronogramaCapilar.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CronogramaCapilar.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Preenche CreatedAtUtc / UpdatedAtUtc de toda raiz de agregado antes de salvar,
/// para ninguém precisar lembrar de fazer isso nos casos de uso.
/// </summary>
public sealed class AuditInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTimestamps(DbContext? context)
    {
        if (context is null)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in context.ChangeTracker.Entries<AggregateRoot>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedAtUtc).CurrentValue = now;
                entry.Property(e => e.UpdatedAtUtc).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified || HasOwnedChanges(entry))
            {
                entry.Property(e => e.UpdatedAtUtc).CurrentValue = now;
            }
        }
    }

    /// <summary>Ex.: só o perfil capilar mudou, mas o usuário também conta como atualizado.</summary>
    private static bool HasOwnedChanges(EntityEntry entry) =>
        entry.References.Any(reference =>
            reference.TargetEntry is { } target
            && target.Metadata.IsOwned()
            && target.State is EntityState.Added or EntityState.Modified);
}
