using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Repositories.Interfaces;
using CronogramaCapilar.Domain.ValueObjects;
using CronogramaCapilar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CronogramaCapilar.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(user => user.Email == email, cancellationToken);

    public void Add(User user) => context.Users.Add(user);

    public void Remove(User user) => context.Users.Remove(user);
}
