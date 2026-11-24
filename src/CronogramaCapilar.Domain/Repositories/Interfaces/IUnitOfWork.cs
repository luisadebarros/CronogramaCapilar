namespace CronogramaCapilar.Domain.Repositories.Interfaces;

/// <summary>
/// Os repositórios só preparam as mudanças; elas vão para o banco todas juntas,
/// numa única transação, quando o caso de uso chama <see cref="SaveChangesAsync"/>.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
