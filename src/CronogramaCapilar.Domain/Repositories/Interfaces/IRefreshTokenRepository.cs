using CronogramaCapilar.Domain.Entities;

namespace CronogramaCapilar.Domain.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> ListActiveByUserAsync(
        Guid userId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    void Add(RefreshToken refreshToken);
}
