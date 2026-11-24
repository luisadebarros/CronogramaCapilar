using CronogramaCapilar.Domain.Common;
using CronogramaCapilar.Domain.Exceptions;

namespace CronogramaCapilar.Domain.Entities;

/// <summary>
/// Sessão do app no celular. Permite pegar um access token novo sem pedir a senha de novo.
/// Guardamos só o hash do token: se o banco vazar, os tokens não podem ser usados.
/// </summary>
public sealed class RefreshToken : AggregateRoot
{
    /// <summary>SHA-256 em hexadecimal.</summary>
    public const int TokenHashLength = 64;

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    /// <summary>Token que substituiu este na rotação (cada uso gera um token novo).</summary>
    public Guid? ReplacedByTokenId { get; private set; }

    public bool WasReplaced => ReplacedByTokenId is not null;

    private RefreshToken() { } // usado pelo EF Core

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
            throw new DomainException("O refresh token precisa pertencer a um usuário.");

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return new RefreshToken { UserId = userId, TokenHash = tokenHash, ExpiresAtUtc = expiresAtUtc };
    }

    public bool IsActive(DateTime nowUtc) => RevokedAtUtc is null && nowUtc < ExpiresAtUtc;

    public void Revoke(DateTime nowUtc) => RevokedAtUtc ??= nowUtc;

    public void ReplaceWith(RefreshToken newToken, DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(newToken);
        if (!IsActive(nowUtc))
            throw new DomainException("Só um refresh token ativo pode ser substituído.");

        RevokedAtUtc = nowUtc;
        ReplacedByTokenId = newToken.Id;
    }
}
