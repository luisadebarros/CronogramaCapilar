using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Domain.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    void Add(User user);

    void Remove(User user);
}
