using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Repositories.Interfaces;
using CronogramaCapilar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CronogramaCapilar.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        context.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> ListActiveByUserAsync(
        Guid userId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default) =>
        await context.RefreshTokens
            .Where(token => token.UserId == userId
                && token.RevokedAtUtc == null
                && token.ExpiresAtUtc > nowUtc)
            .ToListAsync(cancellationToken);

    public void Add(RefreshToken refreshToken) => context.RefreshTokens.Add(refreshToken);
}
