using CronogramaCapilar.Application.Security;
using CronogramaCapilar.Application.Security.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CronogramaCapilar.Infrastructure.Security;

/// <summary>Usa o hasher do ASP.NET Core Identity (PBKDF2 com salt), sem precisar do Identity inteiro.</summary>
public sealed class PasswordHasher : IPasswordHasher
{
    // O hasher do Identity pede um "usuário", mas a implementação padrão não usa esse valor.
    private static readonly object UnusedUser = new();

    private readonly PasswordHasher<object> _hasher = new();
    private readonly string _dummyHash;

    public PasswordHasher() =>
        _dummyHash = _hasher.HashPassword(UnusedUser, Guid.NewGuid().ToString());

    public string Hash(string password) => _hasher.HashPassword(UnusedUser, password);

    public PasswordVerification Verify(string? passwordHash, string password)
    {
        if (passwordHash is null)
        {
            _hasher.VerifyHashedPassword(UnusedUser, _dummyHash, password);
            return PasswordVerification.Failed;
        }

        return _hasher.VerifyHashedPassword(UnusedUser, passwordHash, password) switch
        {
            PasswordVerificationResult.Success => PasswordVerification.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordVerification.SuccessRehashNeeded,
            _ => PasswordVerification.Failed,
        };
    }
}
