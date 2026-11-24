using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Auth;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.Security;
using CronogramaCapilar.Application.Security.Interfaces;
using CronogramaCapilar.Application.Services.Interfaces;
using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Repositories.Interfaces;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    TimeProvider timeProvider) : IAuthService
{
    public async Task<Result<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.Email);
        if (await users.EmailExistsAsync(email, cancellationToken))
            return AuthErrors.EmailInUse;

        // GetValueOrDefault: se algum campo vier vazio, o domínio recusa o valor padrão.
        var user = User.Create(
            request.Name,
            request.PreferredName,
            email,
            passwordHasher.Hash(request.Password),
            request.BirthDate.GetValueOrDefault(),
            request.Gender.GetValueOrDefault(),
            Today());

        users.Add(user);
        var (response, _) = IssueTokens(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = Email.TryCreate(request.Email, out var email)
            ? await users.GetByEmailAsync(email, cancellationToken)
            : null;

        var verification = passwordHasher.Verify(user?.PasswordHash, request.Password);
        if (user is null || verification == PasswordVerification.Failed)
            return AuthErrors.InvalidCredentials;

        if (verification == PasswordVerification.SuccessRehashNeeded)
            user.UpdatePasswordHash(passwordHasher.Hash(request.Password));

        var (response, _) = IssueTokens(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    /// <summary>
    /// Rotação: cada refresh token só pode ser usado uma vez. Usar troca ele por um novo.
    /// Se um token que já foi trocado aparecer de novo, alguém pode ter copiado ele:
    /// derrubamos todas as sessões do usuário por segurança.
    /// </summary>
    public async Task<Result<TokenResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = UtcNow();
        var current = await refreshTokens.GetByHashAsync(tokenService.ComputeHash(request.RefreshToken), cancellationToken);
        if (current is null)
            return AuthErrors.InvalidSession;

        if (!current.IsActive(now))
        {
            if (current.WasReplaced)
            {
                await RevokeAllSessionsAsync(current.UserId, now, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return AuthErrors.InvalidSession;
        }

        var user = await users.GetByIdAsync(current.UserId, cancellationToken);
        if (user is null)
            return AuthErrors.InvalidSession;

        var (response, newRefreshToken) = IssueTokens(user);
        current.ReplaceWith(newRefreshToken, now);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    /// <summary>Encerra a sessão deste aparelho. Sempre responde sucesso, mesmo se o token já não valer.</summary>
    public async Task<Result> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var now = UtcNow();
        var refreshToken = await refreshTokens.GetByHashAsync(tokenService.ComputeHash(request.RefreshToken), cancellationToken);
        if (refreshToken is not null && refreshToken.IsActive(now))
        {
            refreshToken.Revoke(now);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private (TokenResponse Response, RefreshToken RefreshToken) IssueTokens(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var generated = tokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, generated.Hash, generated.ExpiresAtUtc);
        refreshTokens.Add(refreshToken);

        var response = new TokenResponse(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            generated.Token,
            generated.ExpiresAtUtc);

        return (response, refreshToken);
    }

    private async Task RevokeAllSessionsAsync(Guid userId, DateTime now, CancellationToken cancellationToken)
    {
        var activeTokens = await refreshTokens.ListActiveByUserAsync(userId, now, cancellationToken);
        foreach (var token in activeTokens)
            token.Revoke(now);
    }

    private DateTime UtcNow() => timeProvider.GetUtcNow().UtcDateTime;

    private DateOnly Today() => DateOnly.FromDateTime(UtcNow());
}
